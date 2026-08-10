using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int categoryId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int categoryId);
        Task<bool> HasProductsAsync(int categoryId);
    }
}