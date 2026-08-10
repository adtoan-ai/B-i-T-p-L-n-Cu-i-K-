using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    public class ProductController : Controller
    {
        private const int PageSize = 9;

        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBrandRepository _brandRepo;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo, IBrandRepository brandRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
        }

        public static ProductCardVM MapToCard(Product product)
        {
            var activeVariants = product.Variants.Where(v => v.IsActive).ToList();

            var mainImage = activeVariants
                .SelectMany(v => v.Images)
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .FirstOrDefault();

            return new ProductCardVM
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                BrandName = product.Brand?.BrandName ?? string.Empty,
                Style = product.Style,
                MainImageUrl = string.IsNullOrEmpty(mainImage) ? "/images/no-image.png" : mainImage,
                MinPrice = activeVariants.Count > 0 ? activeVariants.Min(v => v.Price) : 0,
                MaxPrice = activeVariants.Count > 0 ? activeVariants.Max(v => v.Price) : 0,
                ColorCount = activeVariants.Count,
                TotalStock = activeVariants.Sum(v => v.StockQuantity)
            };
        }

        public async Task<IActionResult> Index(ProductFilterVM filter)
        {
            if (filter.Page < 1) filter.Page = 1;

            var (items, totalCount) = await _productRepo.SearchAsync(filter, PageSize);

            var model = new ProductListVM
            {
                Products = items.Select(MapToCard).ToList(),
                Filter = filter,
                Categories = await _categoryRepo.GetAllAsync(),
                Brands = await _brandRepo.GetAllAsync(),
                Styles = await _productRepo.GetStylesAsync(),
                Colors = await _productRepo.GetColorsAsync(),
                TotalItems = totalCount,
                CurrentPage = filter.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepo.GetDetailAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductDetailVM
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                BrandName = product.Brand.BrandName,
                CategoryName = product.Category.CategoryName,
                Style = product.Style,
                Material = product.Material,
                Description = product.Description,
                Variants = product.Variants.Select(v => new VariantVM
                {
                    VariantID = v.VariantID,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    ImageUrls = v.Images.Count > 0
                        ? v.Images.Select(i => i.ImageUrl).ToList()
                        : new List<string> { "/images/no-image.png" }
                }).ToList()
            };

            var related = await _productRepo.SearchAsync(
                new ProductFilterVM { BrandId = product.BrandID, Page = 1 }, 8);

            model.RelatedProducts = related.Items
                .Where(p => p.ProductID != id)
                .Take(4)
                .Select(MapToCard)
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Suggest(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
                return Json(new List<object>());

            var products = await _productRepo.SuggestAsync(term.Trim(), 6);

            var result = products.Select(p =>
            {
                var card = MapToCard(p);
                return new
                {
                    id = card.ProductID,
                    name = card.ProductName,
                    brand = card.BrandName,
                    image = card.MainImageUrl,
                    price = card.MinPrice.ToString("#,##0").Replace(",", ".") + " ₫"
                };
            });

            return Json(result);
        }
    }
}