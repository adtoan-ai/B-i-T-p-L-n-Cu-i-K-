using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> SearchAsync(ProductFilterVM filter, int pageSize);
        Task<Product?> GetDetailAsync(int productId);
        Task<List<Product>> GetLatestAsync(int take);
        Task<List<Product>> SuggestAsync(string term, int take);
        Task<List<string>> GetStylesAsync();
        Task<List<string>> GetColorsAsync();
        Task<List<Product>> GetAllForAdminAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int productId);
    }
}