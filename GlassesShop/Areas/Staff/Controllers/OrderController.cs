using GlassesShop.Helpers;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff,Admin")]
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
            ViewBag.IsMyOrders = false;

            var model = orders.Select(OrderService.MapToListItem).ToList();
            return View("Index", model);
        }

        public async Task<IActionResult> MyOrders()
        {
            var staffId = User.GetStaffId();
            var orders = await _orderRepo.GetAllAsync();

            orders = orders.Where(o => o.StaffID == staffId).ToList();

            ViewBag.CurrentStatus = null;
            ViewBag.Keyword = null;
            ViewBag.IsMyOrders = true;

            var model = orders.Select(OrderService.MapToListItem).ToList();
            return View("Index", model);
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
            var validStatuses = new[] { "Đã xác nhận", "Đang giao", "Đã giao" };

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

            if (order.OrderStatus == "Đã giao")
            {
                TempData["Error"] = "Đơn hàng đã giao xong, không thể thay đổi trạng thái.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderRepo.UpdateStatusAsync(id, newStatus, User.GetStaffId());

            TempData["Success"] = $"Đã cập nhật đơn #{id} thành \"{newStatus}\" và ghi nhận bạn phụ trách đơn này.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}