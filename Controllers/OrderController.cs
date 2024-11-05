using MatMatShop.DTOs;
using MatMatShop.Entities;
using MatMatShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MatMatShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context
                .Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Image)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
            return View(orders);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách ảnh chưa bị xóa
            var availableImages = await _context
                .Images.Where(i => !i.IsDeleted)
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Tags = i.Tags
                })
                .ToListAsync();

            ViewBag.AvailableImages = availableImages;
            return View();
        }

        public async Task<IActionResult> FilterImage(string tag)
        {
            var images = await _context
                .Images.Where(i =>
                    (string.IsNullOrEmpty(tag) || i.Tags.Contains(tag)) && !i.IsDeleted
                )
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Tags = i.Tags
                })
                .ToListAsync();
            ViewBag.AvailableImages = images;
            return View("Create"); // Hoặc trả về view tương ứng
        }

        // POST: Order/Create
        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("CustomerName,Phone,Address")] Order order,
            int[] selectedImages
        )
        {
            try
            {
                // Tạo order mới
                _context.Add(order);
                await _context.SaveChangesAsync();

                // Tạo order details cho các ảnh được chọn
                foreach (var imageId in selectedImages)
                {
                    var orderDetail = new OrderDetail { OrderId = order.Id, ImageId = imageId };
                    _context.OrderDetails.Add(orderDetail);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Nếu có lỗi, load lại danh sách ảnh
                var availableImages = await _context
                    .Images.Where(i => !i.IsDeleted)
                    .Select(i => new ImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        Tags = i.Tags
                    })
                    .ToListAsync();

                ViewBag.AvailableImages = availableImages;
                return View(order);
            }
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context
                .Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Image)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,CustomerName,Phone,Address")] Order order
        )
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context
                .Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Image)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
