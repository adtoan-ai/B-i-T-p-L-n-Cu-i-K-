using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands
                .Include(b => b.Products)
                .OrderBy(b => b.BrandName)
                .ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int brandId)
        {
            return await _context.Brands.FindAsync(brandId);
        }

        public async Task AddAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int brandId)
        {
            var brand = await _context.Brands.FindAsync(brandId);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasProductsAsync(int brandId)
        {
            return await _context.Products.AnyAsync(p => p.BrandID == brandId);
        }
    }
}