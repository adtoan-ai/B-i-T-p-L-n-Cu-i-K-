using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.AccountID == accountId);
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Accounts.AnyAsync(a => a.Username == username);
        }

        public async Task<List<Account>> GetByRoleAsync(string role)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Where(a => a.Role == role)
                .OrderByDescending(a => a.AccountID)
                .ToListAsync();
        }

        public async Task CreateCustomerAccountAsync(Account account, Customer customer)
        {
            customer.Cart = new Cart { CreatedAt = DateTime.Now };
            account.Customer = customer;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}