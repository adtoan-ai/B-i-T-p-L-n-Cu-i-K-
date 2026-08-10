namespace GlassesShop.Models.ViewModels
{
    public class ProductCardVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string MainImageUrl { get; set; } = "/images/no-image.png";
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int ColorCount { get; set; }
        public int TotalStock { get; set; }
        public bool HasPriceRange => MaxPrice > MinPrice;
    }
}