namespace GlassesShop.Models.ViewModels
{
    public class ProductFilterVM
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? Style { get; set; }
        public string? Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; } = "newest";
        public int Page { get; set; } = 1;
    }
}