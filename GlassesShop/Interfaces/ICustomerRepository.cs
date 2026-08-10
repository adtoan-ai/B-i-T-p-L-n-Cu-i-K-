using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Account?> GetAccountByCustomerIdAsync(int customerId);
        Task<int> CountOrdersAsync(int customerId);
        Task<Customer?> GetByIdAsync(int customerId);
        Task<Customer?> GetByAccountIdAsync(int accountId);
        Task<List<Customer>> GetAllAsync();
        Task<bool> PhoneExistsAsync(string phone, int? excludeCustomerId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);
        Task UpdateAsync(Customer customer);
    }
}