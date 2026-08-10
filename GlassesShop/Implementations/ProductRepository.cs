using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<Product> BaseQuery()
        {
            return _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                    .ThenInclude(v => v.Images)
                .Where(p => p.IsActive);
        }

        public async Task<(List<Product> Items, int TotalCount)> SearchAsync(ProductFilterVM filter, int pageSize)
        {
            var query = BaseQuery();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                query = query.Where(p => p.ProductName.Contains(keyword)
                                      || p.Brand.BrandName.Contains(keyword)
                                      || p.Style.Contains(keyword));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryID == filter.CategoryId.Value);

            if (filter.BrandId.HasValue)
                query = query.Where(p => p.BrandID == filter.BrandId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Style))
                query = query.Where(p => p.Style == filter.Style);

            if (!string.IsNullOrWhiteSpace(filter.Color))
                query = query.Where(p => p.Variants.Any(v => v.IsActive && v.Color == filter.Color));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Variants.Any(v => v.IsActive && v.Price >= filter.MinPrice.Value));

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Variants.Any(v => v.IsActive && v.Price <= filter.MaxPrice.Value));

            var totalCount = await query.CountAsync();

            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Variants.Where(v => v.IsActive).Min(v => v.Price)),
                "price_desc" => query.OrderByDescending(p => p.Variants.Where(v => v.IsActive).Min(v => v.Price)),
                "name_asc" => query.OrderBy(p => p.ProductName),
                _ => query.OrderByDescending(p => p.ProductID)
            };

            var items = await query
                .Skip((filter.Page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Product?> GetDetailAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                    .ThenInclude(v => v.Images.OrderByDescending(i => i.IsMain).ThenBy(i => i.DisplayOrder))
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.ProductID == productId && p.IsActive);
        }

        public async Task<List<Product>> GetLatestAsync(int take)
        {
            return await BaseQuery()
                .OrderByDescending(p => p.ProductID)
                .Take(take)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Product>> SuggestAsync(string term, int take)
        {
            return await BaseQuery()
                .Where(p => p.ProductName.Contains(term) || p.Brand.BrandName.Contains(term))
                .OrderBy(p => p.ProductName)
                .Take(take)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<string>> GetStylesAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Select(p => p.Style)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<List<string>> GetColorsAsync()
        {
            return await _context.ProductVariants
                .Where(v => v.IsActive)
                .Select(v => v.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllForAdminAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .OrderByDescending(p => p.ProductID)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}