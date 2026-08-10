using GlassesShop.Helpers;
using GlassesShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private int CurrentCustomerId =>
            User.Identity != null && User.Identity.IsAuthenticated ? User.GetCustomerId() : 0;

        public async Task<IActionResult> Index()
        {
            var model = await _cartService.GetCartAsync(CurrentCustomerId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int variantId, int quantity = 1, string? returnUrl = null)
        {
            var (success, message) = await _cartService.AddToCartAsync(CurrentCustomerId, variantId, quantity);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCartAjax(int variantId, int quantity = 1)
        {
            var (success, message) = await _cartService.AddToCartAsync(CurrentCustomerId, variantId, quantity);
            var count = await _cartService.CountAsync(CurrentCustomerId);

            return Json(new { success, message, count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int variantId, int quantity)
        {
            var (success, message) = await _cartService.UpdateQuantityAsync(CurrentCustomerId, variantId, quantity);
            var cart = await _cartService.GetCartAsync(CurrentCustomerId);
            var item = cart.Items.FirstOrDefault(i => i.VariantID == variantId);

            return Json(new
            {
                success,
                message,
                subTotal = item?.SubTotal.ToVnd() ?? "0 ₫",
                totalAmount = cart.TotalAmount.ToVnd(),
                totalQuantity = cart.TotalQuantity,
                quantity = item?.Quantity ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int variantId)
        {
            var (_, message) = await _cartService.RemoveAsync(CurrentCustomerId, variantId);
            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearAsync(CurrentCustomerId);
            TempData["Success"] = "Đã xoá toàn bộ giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var count = await _cartService.CountAsync(CurrentCustomerId);
            return Json(new { count });
        }
    }
}