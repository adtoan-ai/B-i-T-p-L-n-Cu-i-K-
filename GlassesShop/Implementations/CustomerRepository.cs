using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task<Customer?> GetByAccountIdAsync(int accountId)
        {
            return await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.AccountID == accountId);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Account)
                .OrderByDescending(c => c.CustomerID)
                .ToListAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeCustomerId = null)
        {
            return await _context.Customers
                .AnyAsync(c => c.NumberPhone == phone && (excludeCustomerId == null || c.CustomerID != excludeCustomerId));
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email && (excludeCustomerId == null || c.CustomerID != excludeCustomerId));
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        public async Task<Account?> GetAccountByCustomerIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);

            return customer?.Account;
        }

        public async Task<int> CountOrdersAsync(int customerId)
        {
            return await _context.Orders.CountAsync(o => o.CustomerID == customerId);
        }
    }
}