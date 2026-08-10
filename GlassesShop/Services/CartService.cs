using GlassesShop.Helpers;
using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;

namespace GlassesShop.Services
{
    public class CartService
    {
        private const string GuestCartKey = "GuestCart";

        private readonly ICartRepository _cartRepo;
        private readonly IVariantRepository _variantRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ICartRepository cartRepo, IVariantRepository variantRepo, IHttpContextAccessor httpContextAccessor)
        {
            _cartRepo = cartRepo;
            _variantRepo = variantRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        private List<SessionCartItem> GetGuestItems()
        {
            return Session.GetObject<List<SessionCartItem>>(GuestCartKey) ?? new List<SessionCartItem>();
        }

        private void SaveGuestItems(List<SessionCartItem> items)
        {
            Session.SetObject(GuestCartKey, items);
        }

        private static CartItemVM MapToItem(ProductVariant variant, int quantity, int cartDetailId = 0)
        {
            var image = variant.Images
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .FirstOrDefault();

            return new CartItemVM
            {
                CartDetailID = cartDetailId,
                VariantID = variant.VariantID,
                ProductID = variant.ProductID,
                ProductName = variant.Product.ProductName,
                BrandName = variant.Product.Brand?.BrandName ?? string.Empty,
                Color = variant.Color,
                ImageUrl = string.IsNullOrEmpty(image) ? "/images/no-image.png" : image,
                UnitPrice = variant.Price,
                Quantity = quantity,
                StockQuantity = variant.StockQuantity,
                IsAvailable = variant.IsActive && variant.StockQuantity >= quantity
            };
        }

        public async Task<CartVM> GetCartAsync(int customerId)
        {
            if (customerId > 0)
                return await GetMemberCartAsync(customerId);

            return await GetGuestCartAsync();
        }

        private async Task<CartVM> GetGuestCartAsync()
        {
            var model = new CartVM { IsGuestCart = true };
            var items = GetGuestItems();
            if (items.Count == 0) return model;

            var variants = await _variantRepo.GetByIdsAsync(items.Select(i => i.VariantID).ToList());

            foreach (var item in items)
            {
                var variant = variants.FirstOrDefault(v => v.VariantID == item.VariantID);
                if (variant == null) continue;
                model.Items.Add(MapToItem(variant, item.Quantity));
            }

            return model;
        }

        private async Task<CartVM> GetMemberCartAsync(int customerId)
        {
            var model = new CartVM { IsGuestCart = false };
            var cart = await _cartRepo.GetCartWithDetailsAsync(customerId);
            if (cart == null) return model;

            foreach (var detail in cart.CartDetails.OrderBy(d => d.CartDetailID))
            {
                model.Items.Add(MapToItem(detail.Variant, detail.Quantity, detail.CartDetailID));
            }

            return model;
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(int customerId, int variantId, int quantity)
        {
            if (quantity < 1) quantity = 1;

            var variant = await _variantRepo.GetByIdAsync(variantId);
            if (variant == null || !variant.IsActive)
                return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");

            if (variant.StockQuantity <= 0)
                return (false, "Sản phẩm đã hết hàng.");

            if (customerId > 0)
            {
                var cart = await _cartRepo.GetOrCreateCartAsync(customerId);
                var detail = await _cartRepo.GetDetailAsync(cart.CartID, variantId);

                var newQuantity = (detail?.Quantity ?? 0) + quantity;
                if (newQuantity > variant.StockQuantity)
                    return (false, $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho.");

                if (detail == null)
                {
                    await _cartRepo.AddDetailAsync(new CartDetail
                    {
                        CartID = cart.CartID,
                        VariantID = variantId,
                        Quantity = quantity
                    });
                }
                else
                {
                    detail.Quantity = newQuantity;
                    await _cartRepo.UpdateDetailAsync(detail);
                }
            }
            else
            {
                var items = GetGuestItems();
                var existing = items.FirstOrDefault(i => i.VariantID == variantId);
                var newQuantity = (existing?.Quantity ?? 0) + quantity;

                if (newQuantity > variant.StockQuantity)
                    return (false, $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho.");

                if (existing == null)
                    items.Add(new SessionCartItem { VariantID = variantId, Quantity = quantity });
                else
                    existing.Quantity = newQuantity;

                SaveGuestItems(items);
            }

            return (true, $"Đã thêm \"{variant.Product.ProductName} - {variant.Color}\" vào giỏ hàng.");
        }

        public async Task<(bool Success, string Message)> UpdateQuantityAsync(int customerId, int variantId, int quantity)
        {
            if (quantity < 1)
                return await RemoveAsync(customerId, variantId);

            var variant = await _variantRepo.GetByIdAsync(variantId);
            if (variant == null)
                return (false, "Sản phẩm không tồn tại.");

            if (quantity > variant.StockQuantity)
                return (false, $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho.");

            if (customerId > 0)
            {
                var cart = await _cartRepo.GetOrCreateCartAsync(customerId);
                var detail = await _cartRepo.GetDetailAsync(cart.CartID, variantId);
                if (detail == null) return (false, "Sản phẩm không có trong giỏ hàng.");

                detail.Quantity = quantity;
                await _cartRepo.UpdateDetailAsync(detail);
            }
            else
            {
                var items = GetGuestItems();
                var existing = items.FirstOrDefault(i => i.VariantID == variantId);
                if (existing == null) return (false, "Sản phẩm không có trong giỏ hàng.");

                existing.Quantity = quantity;
                SaveGuestItems(items);
            }

            return (true, "Đã cập nhật số lượng.");
        }

        public async Task<(bool Success, string Message)> RemoveAsync(int customerId, int variantId)
        {
            if (customerId > 0)
            {
                var cart = await _cartRepo.GetOrCreateCartAsync(customerId);
                var detail = await _cartRepo.GetDetailAsync(cart.CartID, variantId);
                if (detail != null)
                    await _cartRepo.RemoveDetailAsync(detail.CartDetailID);
            }
            else
            {
                var items = GetGuestItems();
                items.RemoveAll(i => i.VariantID == variantId);
                SaveGuestItems(items);
            }

            return (true, "Đã xoá sản phẩm khỏi giỏ hàng.");
        }

        public async Task ClearAsync(int customerId)
        {
            if (customerId > 0)
            {
                var cart = await _cartRepo.GetOrCreateCartAsync(customerId);
                await _cartRepo.ClearCartAsync(cart.CartID);
            }
            else
            {
                Session.Remove(GuestCartKey);
            }
        }

        public async Task<int> CountAsync(int customerId)
        {
            if (customerId > 0)
                return await _cartRepo.CountItemsAsync(customerId);

            return GetGuestItems().Sum(i => i.Quantity);
        }

        public async Task MergeGuestCartAsync(int customerId)
        {
            var guestItems = GetGuestItems();
            if (guestItems.Count == 0) return;

            var cart = await _cartRepo.GetOrCreateCartAsync(customerId);
            var variants = await _variantRepo.GetByIdsAsync(guestItems.Select(i => i.VariantID).ToList());

            foreach (var item in guestItems)
            {
                var variant = variants.FirstOrDefault(v => v.VariantID == item.VariantID);
                if (variant == null || !variant.IsActive) continue;

                var detail = await _cartRepo.GetDetailAsync(cart.CartID, item.VariantID);
                var newQuantity = (detail?.Quantity ?? 0) + item.Quantity;

                if (newQuantity > variant.StockQuantity)
                    newQuantity = variant.StockQuantity;

                if (newQuantity <= 0) continue;

                if (detail == null)
                {
                    await _cartRepo.AddDetailAsync(new CartDetail
                    {
                        CartID = cart.CartID,
                        VariantID = item.VariantID,
                        Quantity = newQuantity
                    });
                }
                else
                {
                    detail.Quantity = newQuantity;
                    await _cartRepo.UpdateDetailAsync(detail);
                }
            }

            Session.Remove(GuestCartKey);
        }
    }
}