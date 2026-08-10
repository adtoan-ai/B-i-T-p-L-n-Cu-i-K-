using GlassesShop.Helpers;
using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBrandRepository _brandRepo;
        private readonly IVariantRepository _variantRepo;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo,
            IBrandRepository brandRepo, IVariantRepository variantRepo, IWebHostEnvironment env)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
            _variantRepo = variantRepo;
            _env = env;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var products = await _productRepo.GetAllForAdminAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                products = products.Where(p => p.ProductName.ToLower().Contains(k)
                                            || p.Brand.BrandName.ToLower().Contains(k)).ToList();
            }

            ViewBag.Keyword = keyword;

            var model = products.Select(p => new AdminProductListVM
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                CategoryName = p.Category.CategoryName,
                BrandName = p.Brand.BrandName,
                Style = p.Style,
                MainImageUrl = p.Variants
                    .SelectMany(v => v.Images)
                    .OrderByDescending(i => i.IsMain)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault() ?? "/images/no-image.png",
                VariantCount = p.Variants.Count,
                TotalStock = p.Variants.Sum(v => v.StockQuantity),
                MinPrice = p.Variants.Count > 0 ? p.Variants.Min(v => v.Price) : 0,
                IsActive = p.IsActive
            }).ToList();

            return View(model);
        }

        private async Task<ProductFormVM> FillDropdowns(ProductFormVM model)
        {
            model.Categories = await _categoryRepo.GetAllAsync();
            model.Brands = await _brandRepo.GetAllAsync();
            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await FillDropdowns(new ProductFormVM()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM model)
        {
            if (!ModelState.IsValid)
                return View(await FillDropdowns(model));

            var product = new Product
            {
                ProductName = model.ProductName.Trim(),
                CategoryID = model.CategoryID,
                BrandID = model.BrandID,
                Style = model.Style.Trim(),
                Material = model.Material?.Trim(),
                Description = model.Description?.Trim(),
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            await _productRepo.AddAsync(product);

            TempData["Success"] = "Thêm sản phẩm thành công! Hãy thêm biến thể màu sắc cho sản phẩm.";
            return RedirectToAction("Index", "Variant", new { productId = product.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepo.GetDetailAsync(id);
            if (product == null)
            {
                var all = await _productRepo.GetAllForAdminAsync();
                product = all.FirstOrDefault(p => p.ProductID == id);
            }

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductFormVM
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                BrandID = product.BrandID,
                Style = product.Style,
                Material = product.Material,
                Description = product.Description,
                IsActive = product.IsActive
            };

            return View(await FillDropdowns(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormVM model)
        {
            if (!ModelState.IsValid)
                return View(await FillDropdowns(model));

            var all = await _productRepo.GetAllForAdminAsync();
            var product = all.FirstOrDefault(p => p.ProductID == model.ProductID);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            product.ProductName = model.ProductName.Trim();
            product.CategoryID = model.CategoryID;
            product.BrandID = model.BrandID;
            product.Style = model.Style.Trim();
            product.Material = model.Material?.Trim();
            product.Description = model.Description?.Trim();
            product.IsActive = model.IsActive;

            await _productRepo.UpdateAsync(product);

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var all = await _productRepo.GetAllForAdminAsync();
            var product = all.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var variant in product.Variants)
            {
                foreach (var image in variant.Images)
                    FileHelper.DeleteImage(image.ImageUrl, _env.WebRootPath);
            }

            try
            {
                await _productRepo.DeleteAsync(id);
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa sản phẩm này vì đã có đơn hàng liên quan. Hãy chuyển sang trạng thái ngừng kinh doanh.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var all = await _productRepo.GetAllForAdminAsync();
            var product = all.FirstOrDefault(p => p.ProductID == id);

            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _productRepo.UpdateAsync(product);
                TempData["Success"] = product.IsActive
                    ? "Đã mở bán sản phẩm."
                    : "Đã ngừng kinh doanh sản phẩm.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}