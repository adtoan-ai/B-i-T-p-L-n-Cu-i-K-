using GlassesShop.Models.Entities;

namespace GlassesShop.Models.ViewModels
{
    public class ProductListVM
    {
        public List<ProductCardVM> Products { get; set; } = new();
        public ProductFilterVM Filter { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<string> Styles { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}