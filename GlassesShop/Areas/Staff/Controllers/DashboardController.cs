using GlassesShop.Helpers;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff,Admin")]
    public class DashboardController : Controller
    {
        private readonly IOrderRepository _orderRepo;

        public DashboardController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllAsync();
            var staffId = User.GetStaffId();

            var model = new DashboardVM
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.OrderStatus == "Chờ xác nhận"),
                CancelledOrders = orders.Count(o => o.OrderStatus == "Đã hủy"),
                TotalRevenue = orders.Where(o => o.OrderStatus == "Đã giao").Sum(o => o.TotalAmount),
                RecentOrders = orders.Take(10).Select(OrderService.MapToListItem).ToList()
            };

            ViewBag.MyOrderCount = orders.Count(o => o.StaffID == staffId);
            ViewBag.ShippingCount = orders.Count(o => o.OrderStatus == "Đang giao");

            return View(model);
        }
    }
}