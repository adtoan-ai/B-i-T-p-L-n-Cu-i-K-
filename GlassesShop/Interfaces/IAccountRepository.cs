using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<Account?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<List<Account>> GetByRoleAsync(string role);
        Task CreateCustomerAccountAsync(Account account, Customer customer);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int accountId);
    }
}