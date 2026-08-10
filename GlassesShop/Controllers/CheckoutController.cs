using GlassesShop.Helpers;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly MoMoService _moMoService;

        public CheckoutController(CartService cartService, OrderService orderService,
            ICustomerRepository customerRepo, IOrderRepository orderRepo, MoMoService moMoService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
            _moMoService = moMoService;
        }

        public async Task<IActionResult> Index()
        {
            var customerId = User.GetCustomerId();
            var cart = await _cartService.GetCartAsync(customerId);

            if (cart.Items.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var customer = await _customerRepo.GetByIdAsync(customerId);

            var model = new CheckoutVM
            {
                ReceiverName = customer?.FullName ?? string.Empty,
                ReceiverPhone = customer?.NumberPhone ?? string.Empty,
                ShippingAddress = customer?.Address ?? string.Empty,
                PaymentMethod = "COD",
                Cart = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutVM model)
        {
            var customerId = User.GetCustomerId();
            var validMethods = new[] { "COD", "MoMo" };

            if (Array.IndexOf(validMethods, model.PaymentMethod) < 0)
                ModelState.AddModelError(nameof(model.PaymentMethod), "Phương thức thanh toán không hợp lệ.");

            if (!ModelState.IsValid)
            {
                model.Cart = await _cartService.GetCartAsync(customerId);
                return View(model);
            }

            var (success, message, orderId) = await _orderService.PlaceOrderAsync(customerId, model);

            if (!success)
            {
                TempData["Error"] = message;
                return RedirectToAction("Index", "Cart");
            }

            if (model.PaymentMethod == "MoMo")
                return await RedirectToMoMo(orderId);

            TempData["Success"] = "Đặt hàng thành công! Đơn hàng của bạn đang chờ xác nhận.";
            return RedirectToAction(nameof(Success), new { id = orderId });
        }

        private async Task<IActionResult> RedirectToMoMo(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return RedirectToAction("Index", "Home");

            var redirectUrl = Url.Action(nameof(MomoReturn), "Checkout", new { orderId }, Request.Scheme)!;
            var ipnUrl = $"{Request.Scheme}://{Request.Host}/api/momo-ipn";

            var result = await _moMoService.CreatePaymentAsync(
                order.OrderID, order.TotalAmount, $"Thanh toan don hang #{order.OrderID}", redirectUrl, ipnUrl);

            if (!result.Success || string.IsNullOrEmpty(result.PayUrl))
            {
                TempData["Error"] = "Không thể khởi tạo thanh toán MoMo: " + result.Message;
                return RedirectToAction(nameof(PaymentGateway), new { id = orderId });
            }

            return Redirect(result.PayUrl);
        }

        public IActionResult MomoReturn(int orderId)
        {
            return RedirectToAction(nameof(PaymentGateway), new { id = orderId });
        }

        public async Task<IActionResult> PaymentGateway(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null || order.CustomerID != User.GetCustomerId())
                return RedirectToAction("Index", "Home");

            if (order.Payment != null && order.Payment.PaymentStatus == "Đã thanh toán")
                return RedirectToAction(nameof(Success), new { id });

            return View("PaymentGatewayMomo", OrderService.MapToDetail(order));
        }

        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null || order.CustomerID != User.GetCustomerId())
                return Json(new { paymentStatus = "not_found" });

            return Json(new { paymentStatus = order.Payment?.PaymentStatus ?? "Chưa thanh toán" });
        }

        public async Task<IActionResult> Success(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null || order.CustomerID != User.GetCustomerId())
                return RedirectToAction("Index", "Home");

            return View(OrderService.MapToDetail(order));
        }
    }
}