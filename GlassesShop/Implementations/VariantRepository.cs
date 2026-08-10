using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class VariantRepository : IVariantRepository
    {
        private readonly ApplicationDbContext _context;

        public VariantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariant?> GetByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                    .ThenInclude(p => p.Brand)
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.VariantID == variantId);
        }

        public async Task<List<ProductVariant>> GetByIdsAsync(List<int> variantIds)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                    .ThenInclude(p => p.Brand)
                .Include(v => v.Images)
                .Where(v => variantIds.Contains(v.VariantID))
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(v => v.Images)
                .Where(v => v.ProductID == productId)
                .OrderBy(v => v.Color)
                .ToListAsync();
        }

        public async Task AddAsync(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int variantId)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ColorExistsAsync(int productId, string color, int? excludeVariantId = null)
        {
            return await _context.ProductVariants.AnyAsync(v =>
                v.ProductID == productId &&
                v.Color == color &&
                (excludeVariantId == null || v.VariantID != excludeVariantId));
        }

        public async Task UpdateStockAsync(int variantId, int quantityChange)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                variant.StockQuantity += quantityChange;
                if (variant.StockQuantity < 0) variant.StockQuantity = 0;
                await _context.SaveChangesAsync();
            }
        }
    }
}