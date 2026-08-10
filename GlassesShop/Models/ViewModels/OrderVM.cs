namespace GlassesShop.Models.ViewModels
{
    public class OrderItemVM
    {
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string ImageUrl { get; set; } = "/images/no-image.png";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public class OrderListItemVM
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public int ItemCount { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public bool CanCancel => OrderStatus == "Chờ xác nhận" || OrderStatus == "Đã xác nhận";
    }

    public class OrderDetailVM
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? Note { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? StaffName { get; set; }
        public List<OrderItemVM> Items { get; set; } = new();
        public bool CanCancel => OrderStatus == "Chờ xác nhận" || OrderStatus == "Đã xác nhận";
    }
}