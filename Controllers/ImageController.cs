using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MatMatShop.Infrastructure;
using MatMatShop.Infrastructure.Hubs;
using MatMatShop.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Image = MatMatShop.Entities.Image;

namespace MatMatShop.Controllers
{
    public class ImageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ImageHub, IImageClient> _hubContext;
        private readonly IWebHostEnvironment _environment;

        public ImageController(
            AppDbContext context,
            IHubContext<ImageHub, IImageClient> hubContext,
            IWebHostEnvironment environment
        )
        {
            _context = context;
            _hubContext = hubContext;
            _environment = environment;
        }

        // Hiển thị danh sách ảnh
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var images = await _context.Images.Where(i => !i.IsDeleted).ToListAsync();
            return View(images);
        }

        // Hiển thị trang upload ảnh
        public IActionResult Upload()
        {
            return View();
        }

        // Xử lý việc upload ảnh
        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, string tags)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("No files were uploaded.");
            }

            var uploadedImages = new List<Image>();
            var tagList = tags.Split(',').Select(t => t.Trim()).ToList();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Generate a unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Create and save the image entity
                    var image = new Image
                    {
                        ImageUrl = "/images/" + fileName,
                        Tags = string.Join(", ", tagList),
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();

                    uploadedImages.Add(image);

                    // Send SignalR notification for each image
                    await _hubContext.Clients.All.ReceiveNewImage(
                        image.Id,
                        image.ImageUrl,
                        image.Tags
                    );
                }
            }

            // Redirect to Index action after all images are processed
            return RedirectToAction("Index");
        }

        // Lọc ảnh theo tag
        public async Task<IActionResult> Filter(string tag)
        {
            var images = await _context
                .Images.Where(i =>
                    (string.IsNullOrEmpty(tag) || i.Tags.Contains(tag)) && !i.IsDeleted
                )
                .ToListAsync();
            return View("Index", images);
        }

        // Chuyển ảnh vào thùng rác
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                image.IsDeleted = true;
                await _context.SaveChangesAsync();
                // Phát tín hiệu cho các client về việc xóa ảnh
                await _hubContext.Clients.All.ReceiveDeleteImage(id, image.ImageUrl, image.Tags);
            }
            return RedirectToAction("Index");
        }

        // Khôi phục ảnh từ thùng rác
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                image.IsDeleted = false;
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.ReceiveRestoreImage(
                    image.Id,
                    image.ImageUrl,
                    image.Tags
                );
            }
            return RedirectToAction("Trash");
        }

        // Xóa ảnh vĩnh viễn
        [HttpPost]
        public async Task<IActionResult> PermanentlyDelete(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                var fileName = image.ImageUrl; // Assuming ImageUrl contains the relative path like "images/yourimage.jpg"

                // Get the absolute path to the wwwroot folder
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Combine wwwroot path with image path
                var filePath = Path.Combine(wwwRootPath, fileName);

                // Remove the image record from the database
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                // Delete the image file from wwwroot
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            return RedirectToAction("Trash");
        }

        // Hiển thị thùng rác
        public async Task<IActionResult> Trash()
        {
            var images = await _context.Images.Where(i => i.IsDeleted).ToListAsync();
            return View(images);
        }
    }
}
