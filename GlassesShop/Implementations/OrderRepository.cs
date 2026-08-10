using GlassesShop.Data;
using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateOrderAsync(Order order, List<OrderDetail> details, Payment payment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var detail in details)
                {
                    detail.OrderID = order.OrderID;
                    _context.OrderDetails.Add(detail);

                    var variant = await _context.ProductVariants.FindAsync(detail.VariantID);
                    if (variant == null)
                        throw new InvalidOperationException("Sản phẩm không tồn tại.");

                    if (variant.StockQuantity < detail.Quantity)
                        throw new InvalidOperationException($"Sản phẩm \"{variant.Color}\" không đủ số lượng tồn kho.");

                    variant.StockQuantity -= detail.Quantity;
                }

                payment.OrderID = order.OrderID;
                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.OrderID;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.Payment)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Brand)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<List<Order>> GetByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderID)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllAsync(string? status = null, string? keyword = null)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.Payment)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.OrderStatus == status);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                query = query.Where(o => o.ReceiverName.Contains(k)
                                      || o.ReceiverPhone.Contains(k)
                                      || o.Customer.FullName.Contains(k));
            }

            return await query
                .OrderByDescending(o => o.OrderID)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string newStatus, int? staffId = null)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null) return false;

            order.OrderStatus = newStatus;

            if (staffId.HasValue && staffId.Value > 0)
                order.StaffID = staffId.Value;

            if (newStatus == "Đã giao" && order.Payment != null
                && order.Payment.PaymentStatus == "Chưa thanh toán")
            {
                order.Payment.PaymentStatus = "Đã thanh toán";
                order.Payment.PaidAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Payment)
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);

                if (order == null) return false;

                foreach (var detail in order.OrderDetails)
                {
                    var variant = await _context.ProductVariants.FindAsync(detail.VariantID);
                    if (variant != null)
                        variant.StockQuantity += detail.Quantity;
                }

                order.OrderStatus = "Đã hủy";

                if (order.Payment != null && order.Payment.PaymentStatus == "Đã thanh toán")
                    order.Payment.PaymentStatus = "Đã hoàn tiền";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus, string? transactionCode = null)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == orderId);
            if (payment == null) return false;

            payment.PaymentStatus = paymentStatus;

            if (!string.IsNullOrEmpty(transactionCode))
                payment.TransactionCode = transactionCode;

            if (paymentStatus == "Đã thanh toán")
                payment.PaidAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}