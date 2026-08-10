using GlassesShop.Models.Entities;

namespace GlassesShop.Models.ViewModels
{
    public class HomeVM
    {
        public List<ProductCardVM> LatestProducts { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
    }
}