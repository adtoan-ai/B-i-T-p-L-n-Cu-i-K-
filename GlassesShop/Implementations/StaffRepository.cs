using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .Include(s => s.Orders)
                .OrderByDescending(s => s.StaffID)
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(int staffId)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.StaffID == staffId);
        }

        public async Task CreateStaffAccountAsync(Account account, Staff staff)
        {
            account.Staff = staff;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int staffId)
        {
            var staff = await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.StaffID == staffId);

            if (staff != null)
            {
                _context.Accounts.Remove(staff.Account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeStaffId = null)
        {
            return await _context.Staffs
                .AnyAsync(s => s.Email == email && (excludeStaffId == null || s.StaffID != excludeStaffId));
        }

        public async Task<bool> HasOrdersAsync(int staffId)
        {
            return await _context.Orders.AnyAsync(o => o.StaffID == staffId);
        }
    }
}