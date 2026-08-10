namespace GlassesShop.Models.ViewModels
{
    public class RevenueStatisticsVM
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }

        public List<TopProductVM> TopProducts { get; set; } = new();
    }

    public class TopProductVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}