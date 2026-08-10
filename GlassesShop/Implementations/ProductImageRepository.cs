using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductImage>> GetByVariantAsync(int variantId)
        {
            return await _context.ProductImages
                .Where(i => i.VariantID == variantId)
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(int imageId)
        {
            return await _context.ProductImages.FindAsync(imageId);
        }

        public async Task AddRangeAsync(List<ProductImage> images)
        {
            _context.ProductImages.AddRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetMainAsync(int variantId, int imageId)
        {
            var images = await _context.ProductImages
                .Where(i => i.VariantID == variantId)
                .ToListAsync();

            foreach (var image in images)
                image.IsMain = image.ImageID == imageId;

            await _context.SaveChangesAsync();
        }
    }
}