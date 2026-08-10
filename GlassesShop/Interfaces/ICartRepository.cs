using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateCartAsync(int customerId);
        Task<Cart?> GetCartWithDetailsAsync(int customerId);
        Task<CartDetail?> GetDetailAsync(int cartId, int variantId);
        Task AddDetailAsync(CartDetail detail);
        Task UpdateDetailAsync(CartDetail detail);
        Task RemoveDetailAsync(int cartDetailId);
        Task ClearCartAsync(int cartId);
        Task<int> CountItemsAsync(int customerId);
    }
}