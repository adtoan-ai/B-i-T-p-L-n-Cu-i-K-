using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index(string? status, string? keyword)
        {
            var orders = await _orderRepo.GetAllAsync(status, keyword);

            ViewBag.CurrentStatus = status;
            ViewBag.Keyword = keyword;

            var model = orders.Select(OrderService.MapToListItem).ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(OrderService.MapToDetail(order));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var validStatuses = new[] { "Chờ xác nhận", "Đã xác nhận", "Đang giao", "Đã giao" };

            if (!validStatuses.Contains(newStatus))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (order.OrderStatus == "Đã hủy")
            {
                TempData["Error"] = "Đơn hàng đã hủy, không thể cập nhật trạng thái.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderRepo.UpdateStatusAsync(id, newStatus);
            TempData["Success"] = $"Đã cập nhật trạng thái đơn #{id} thành \"{newStatus}\".";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (order.OrderStatus == "Đã hủy" || order.OrderStatus == "Đã giao")
            {
                TempData["Error"] = "Đơn hàng này không thể hủy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderRepo.CancelOrderAsync(id);
            TempData["Success"] = $"Đã hủy đơn hàng #{id}. Tồn kho đã được hoàn lại.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}