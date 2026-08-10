using GlassesShop.Models.Entities;

namespace GlassesShop.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(Order order, List<OrderDetail> details, Payment payment);
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetByCustomerAsync(int customerId);
        Task<List<Order>> GetAllAsync(string? status = null, string? keyword = null);
        Task<bool> UpdateStatusAsync(int orderId, string newStatus, int? staffId = null);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus, string? transactionCode = null);
    }
}