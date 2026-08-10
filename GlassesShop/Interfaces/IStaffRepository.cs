using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<List<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(int staffId);
        Task CreateStaffAccountAsync(Account account, Staff staff);
        Task UpdateAsync(Staff staff);
        Task DeleteAsync(int staffId);
        Task<bool> EmailExistsAsync(string email, int? excludeStaffId = null);
        Task<bool> HasOrdersAsync(int staffId);
    }
}