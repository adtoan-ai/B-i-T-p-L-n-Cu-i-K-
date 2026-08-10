using GlassesShop.Data;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepo;

        public DashboardController(ApplicationDbContext context, IOrderRepository orderRepo)
        {
            _context = context;
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllAsync();

            var lowStock = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.IsActive && v.StockQuantity <= 5)
                .OrderBy(v => v.StockQuantity)
                .Take(8)
                .Select(v => new LowStockItemVM
                {
                    ProductID = v.ProductID,
                    ProductName = v.Product.ProductName,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                })
                .ToListAsync();

            var model = new DashboardVM
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalVariants = await _context.ProductVariants.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalStaffs = await _context.Staffs.CountAsync(),
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.OrderStatus == "Chờ xác nhận"),
                CancelledOrders = orders.Count(o => o.OrderStatus == "Đã hủy"),
                TotalRevenue = orders.Where(o => o.OrderStatus == "Đã giao").Sum(o => o.TotalAmount),
                LowStockCount = lowStock.Count,
                LowStockItems = lowStock,
                RecentOrders = orders.Take(8).Select(OrderService.MapToListItem).ToList()
            };

            return View(model);
        }
        
        public async Task<IActionResult> Statistics(DateTime? fromDate, DateTime? toDate)
        {
            var to = (toDate ?? DateTime.Today).Date;
            var from = (fromDate ?? to.AddDays(-29)).Date;

         
            if (from > to)
                (from, to) = (to, from);

            var toExclusive = to.AddDays(1); 

            var deliveredOrdersQuery = _context.Orders
                .Where(o => o.OrderStatus == "Đã giao"
                         && o.OrderDate >= from
                         && o.OrderDate < toExclusive);

            var totalRevenue = await deliveredOrdersQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
            var totalOrders = await deliveredOrdersQuery.CountAsync();

            var deliveredDetailsQuery = _context.OrderDetails
                .Where(od => od.Order.OrderStatus == "Đã giao"
                          && od.Order.OrderDate >= from
                          && od.Order.OrderDate < toExclusive);

            var totalProductsSold = await deliveredDetailsQuery.SumAsync(od => (int?)od.Quantity) ?? 0;

            var topProducts = await deliveredDetailsQuery
                .GroupBy(od => new { od.Variant.ProductID, od.Variant.Product.ProductName })
                .Select(g => new TopProductVM
                {
                    ProductID = g.Key.ProductID,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            var model = new RevenueStatisticsVM
            {
                FromDate = from,
                ToDate = to,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                TopProducts = topProducts
            };

            return View(model);
        }
    }
}