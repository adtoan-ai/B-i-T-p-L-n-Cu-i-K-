using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    public class ChatBotController : Controller
    {
        private const int MaxResults = 6;

        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBrandRepository _brandRepo;

        public ChatBotController(IProductRepository productRepo, ICategoryRepository categoryRepo, IBrandRepository brandRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetOptions()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var brands = await _brandRepo.GetAllAsync();
            var styles = await _productRepo.GetStylesAsync();
            var colors = await _productRepo.GetColorsAsync();

            var result = new
            {
                categories = categories.Select(c => new { id = c.CategoryID, name = c.CategoryName }),
                brands = brands.Select(b => new { id = b.BrandID, name = b.BrandName }),
                styles,
                colors
            };

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] ChatBotFilterVM filter)
        {
            var searchFilter = new ProductFilterVM
            {
                CategoryId = filter.CategoryId,
                BrandId = filter.BrandId,
                Style = filter.Style,
                Color = filter.Color,
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                SortBy = "newest",
                Page = 1
            };

            var (items, totalCount) = await _productRepo.SearchAsync(searchFilter, MaxResults);

            var products = items.Select(p =>
            {
                var card = ProductController.MapToCard(p);
                return new
                {
                    id = card.ProductID,
                    name = card.ProductName,
                    brand = card.BrandName,
                    style = card.Style,
                    image = card.MainImageUrl,
                    minPrice = card.MinPrice,
                    maxPrice = card.MaxPrice,
                    priceText = card.HasPriceRange
                        ? $"{card.MinPrice:#,##0}".Replace(",", ".") + " ₫ - " + $"{card.MaxPrice:#,##0}".Replace(",", ".") + " ₫"
                        : $"{card.MinPrice:#,##0}".Replace(",", ".") + " ₫"
                };
            }).ToList();

            return Json(new
            {
                totalCount,
                products
            });
        }
    }
}