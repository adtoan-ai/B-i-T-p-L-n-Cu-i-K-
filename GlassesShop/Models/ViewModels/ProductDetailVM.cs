namespace GlassesShop.Models.ViewModels
{
    public class VariantVM
    {
        public int VariantID { get; set; }
        public string Color { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

    public class ProductDetailVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string? Material { get; set; }
        public string? Description { get; set; }
        public List<VariantVM> Variants { get; set; } = new();
        public List<ProductCardVM> RelatedProducts { get; set; } = new();
    }
}