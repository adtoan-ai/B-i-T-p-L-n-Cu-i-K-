using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IVariantRepository
    {
        Task<ProductVariant?> GetByIdAsync(int variantId);
        Task<List<ProductVariant>> GetByIdsAsync(List<int> variantIds);
        Task<List<ProductVariant>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductVariant variant);
        Task UpdateAsync(ProductVariant variant);
        Task DeleteAsync(int variantId);
        Task<bool> ColorExistsAsync(int productId, string color, int? excludeVariantId = null);
        Task UpdateStockAsync(int variantId, int quantityChange);
    }
}