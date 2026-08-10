namespace GlassesShop.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalProducts { get; set; }
        public int TotalVariants { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalStaffs { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockCount { get; set; }
        public List<OrderListItemVM> RecentOrders { get; set; } = new();
        public List<LowStockItemVM> LowStockItems { get; set; } = new();
    }

    public class LowStockItemVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int StockQuantity { get; set; }
    }
}