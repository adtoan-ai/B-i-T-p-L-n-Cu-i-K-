namespace GlassesShop.Models.ViewModels
{
    public class SessionCartItem
    {
        public int VariantID { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemVM
    {
        public int CartDetailID { get; set; }
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string ImageUrl { get; set; } = "/images/no-image.png";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public bool IsGuestCart { get; set; }
        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal TotalAmount => Items.Where(i => i.IsAvailable).Sum(i => i.SubTotal);
        public bool HasInvalidItem => Items.Any(i => !i.IsAvailable);
    }
}