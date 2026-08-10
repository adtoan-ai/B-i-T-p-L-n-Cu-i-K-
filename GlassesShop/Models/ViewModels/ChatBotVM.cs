namespace GlassesShop.Models.ViewModels
{
    
    public class ChatBotFilterVM
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? Style { get; set; }
        public string? Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}