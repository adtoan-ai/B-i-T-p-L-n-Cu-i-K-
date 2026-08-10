using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(int customerId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerID == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<Cart?> GetCartWithDetailsAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Brand)
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task<CartDetail?> GetDetailAsync(int cartId, int variantId)
        {
            return await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.CartID == cartId && cd.VariantID == variantId);
        }

        public async Task AddDetailAsync(CartDetail detail)
        {
            _context.CartDetails.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDetailAsync(CartDetail detail)
        {
            _context.CartDetails.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDetailAsync(int cartDetailId)
        {
            var detail = await _context.CartDetails.FindAsync(cartDetailId);
            if (detail != null)
            {
                _context.CartDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var details = await _context.CartDetails.Where(cd => cd.CartID == cartId).ToListAsync();
            if (details.Count > 0)
            {
                _context.CartDetails.RemoveRange(details);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountItemsAsync(int customerId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerID == customerId);
            if (cart == null) return 0;

            return await _context.CartDetails
                .Where(cd => cd.CartID == cart.CartID)
                .SumAsync(cd => (int?)cd.Quantity) ?? 0;
        }
    }
}