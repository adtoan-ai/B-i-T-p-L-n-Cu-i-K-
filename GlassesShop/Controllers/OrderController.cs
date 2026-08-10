using GlassesShop.Helpers;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var orders = await _orderRepo.GetByCustomerAsync(User.GetCustomerId());

            if (!string.IsNullOrWhiteSpace(status))
                orders = orders.Where(o => o.OrderStatus == status).ToList();

            ViewBag.CurrentStatus = status;

            var model = orders.Select(OrderService.MapToListItem).ToList();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null || order.CustomerID != User.GetCustomerId())
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(OrderService.MapToDetail(order));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null || order.CustomerID != User.GetCustomerId())
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (order.OrderStatus != "Chờ xác nhận" && order.OrderStatus != "Đã xác nhận")
            {
                TempData["Error"] = "Đơn hàng đang được giao hoặc đã hoàn tất, không thể hủy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _orderRepo.CancelOrderAsync(id);
            TempData["Success"] = $"Đã hủy đơn hàng #{id}. Sản phẩm đã được hoàn lại kho.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}