using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByVariantAsync(int variantId);
        Task<ProductImage?> GetByIdAsync(int imageId);
        Task AddRangeAsync(List<ProductImage> images);
        Task DeleteAsync(int imageId);
        Task SetMainAsync(int variantId, int imageId);
    }
}