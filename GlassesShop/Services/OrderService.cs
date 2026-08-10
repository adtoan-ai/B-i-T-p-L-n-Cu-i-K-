using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;

namespace GlassesShop.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly CartService _cartService;

        public OrderService(IOrderRepository orderRepo, CartService cartService)
        {
            _orderRepo = orderRepo;
            _cartService = cartService;
        }

        public async Task<(bool Success, string Message, int OrderId)> PlaceOrderAsync(int customerId, CheckoutVM model)
        {
            var cart = await _cartService.GetCartAsync(customerId);

            if (cart.Items.Count == 0)
                return (false, "Giỏ hàng của bạn đang trống.", 0);

            if (cart.HasInvalidItem)
                return (false, "Có sản phẩm không đủ hàng trong kho. Vui lòng kiểm tra lại giỏ hàng.", 0);

            var order = new Order
            {
                CustomerID = customerId,
                OrderDate = DateTime.Now,
                ReceiverName = model.ReceiverName.Trim(),
                ReceiverPhone = model.ReceiverPhone.Trim(),
                ShippingAddress = model.ShippingAddress.Trim(),
                Note = model.Note?.Trim(),
                TotalAmount = cart.TotalAmount,
                OrderStatus = "Chờ xác nhận"
            };

            var details = cart.Items.Select(i => new OrderDetail
            {
                VariantID = i.VariantID,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var payment = new Payment
            {
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Chưa thanh toán",
                Amount = cart.TotalAmount
            };

            try
            {
                var orderId = await _orderRepo.CreateOrderAsync(order, details, payment);
                await _cartService.ClearAsync(customerId);
                return (true, "Đặt hàng thành công!", orderId);
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message, 0);
            }
            catch
            {
                return (false, "Có lỗi xảy ra khi tạo đơn hàng. Vui lòng thử lại.", 0);
            }
        }

        public static OrderListItemVM MapToListItem(Order order)
        {
            return new OrderListItemVM
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.Payment?.PaymentMethod ?? "COD",
                PaymentStatus = order.Payment?.PaymentStatus ?? "Chưa thanh toán",
                ItemCount = order.OrderDetails.Count,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                ReceiverPhone = order.ReceiverPhone
            };
        }

        public static OrderDetailVM MapToDetail(Order order)
        {
            return new OrderDetailVM
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.Payment?.PaymentMethod ?? "COD",
                PaymentStatus = order.Payment?.PaymentStatus ?? "Chưa thanh toán",
                TransactionCode = order.Payment?.TransactionCode,
                PaidAt = order.Payment?.PaidAt,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                CustomerEmail = order.Customer?.Email ?? string.Empty,
                StaffName = order.Staff?.FullName,
                Items = order.OrderDetails.Select(od => new OrderItemVM
                {
                    VariantID = od.VariantID,
                    ProductID = od.Variant.ProductID,
                    ProductName = od.Variant.Product.ProductName,
                    BrandName = od.Variant.Product.Brand?.BrandName ?? string.Empty,
                    Color = od.Variant.Color,
                    ImageUrl = od.Variant.Images
                        .OrderByDescending(i => i.IsMain)
                        .ThenBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault() ?? "/images/no-image.png",
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity
                }).ToList()
            };
        }

        public static string GetStatusBadgeClass(string status) => status switch
        {
            "Chờ xác nhận" => "bg-warning text-dark",
            "Đã xác nhận" => "bg-info text-dark",
            "Đang giao" => "bg-primary",
            "Đã giao" => "bg-success",
            "Đã hủy" => "bg-danger",
            _ => "bg-secondary"
        };

        public static string GetPaymentBadgeClass(string status) => status switch
        {
            "Đã thanh toán" => "bg-success",
            "Chưa thanh toán" => "bg-warning text-dark",
            "Thất bại" => "bg-danger",
            "Đã hoàn tiền" => "bg-secondary",
            _ => "bg-secondary"
        };
    }
}