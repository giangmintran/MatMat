using MatMatShop.Entities;
using MatMatShop.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MatMatShop.Infrastructure.Hubs;
using static System.Net.Mime.MediaTypeNames;
using Image = MatMatShop.Entities.Image;

namespace MatMatShop.Controllers
{
    public class ImageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ImageHub> _hubContext;
        public ImageController(AppDbContext context, IHubContext<ImageHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
        public async Task<IActionResult> Upload(IFormFile file, List<string> tags)
        {
            // Lưu ảnh vào server
            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new Image
            {
                ImageUrl = "/images/" + fileName,
                Tags = string.Join(", ", tags),
                CreatedDate = DateTime.Now
            };

            // Lưu vào database
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            // Phát tín hiệu cho các client về ảnh mới
            await _hubContext.Clients.All.SendAsync("ReceiveNewImage", image.Id, image.ImageUrl, image.Tags);

            return RedirectToAction("Index");
        }

        // Lọc ảnh theo tag
        public async Task<IActionResult> Filter(string tag)
        {
            var images = await _context.Images
                .Where(i => i.Tags.Contains(tag) && !i.IsDeleted)
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
                await _hubContext.Clients.All.SendAsync("ReceiveDeleteImage", id, image.ImageUrl, image.Tags);
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

                await _hubContext.Clients.All.SendAsync("ReceiveNewImage", image.Id, image.ImageUrl, image.Tags);
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
                var filePath = image.ImageUrl;
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
                System.IO.File.Delete(filePath);
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
