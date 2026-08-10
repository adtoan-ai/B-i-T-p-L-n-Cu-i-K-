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
    public class VariantController : Controller
    {
        private readonly IVariantRepository _variantRepo;
        private readonly IProductRepository _productRepo;
        private readonly IProductImageRepository _imageRepo;
        private readonly IWebHostEnvironment _env;

        public VariantController(IVariantRepository variantRepo, IProductRepository productRepo,
            IProductImageRepository imageRepo, IWebHostEnvironment env)
        {
            _variantRepo = variantRepo;
            _productRepo = productRepo;
            _imageRepo = imageRepo;
            _env = env;
        }

        public async Task<IActionResult> Index(int productId)
        {
            var all = await _productRepo.GetAllForAdminAsync();
            var product = all.FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index", "Product");
            }

            ViewBag.ProductName = product.ProductName;
            ViewBag.ProductID = productId;

            var variants = await _variantRepo.GetByProductIdAsync(productId);
            return View(variants);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            var all = await _productRepo.GetAllForAdminAsync();
            var product = all.FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index", "Product");
            }

            return View(new VariantFormVM
            {
                ProductID = productId,
                ProductName = product.ProductName,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VariantFormVM model, List<IFormFile>? images)
        {
            if (await _variantRepo.ColorExistsAsync(model.ProductID, model.Color.Trim()))
                ModelState.AddModelError(nameof(model.Color), "Màu sắc này đã tồn tại cho sản phẩm.");

            if (!ModelState.IsValid)
                return View(model);

            var variant = new ProductVariant
            {
                ProductID = model.ProductID,
                Color = model.Color.Trim(),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive
            };

            await _variantRepo.AddAsync(variant);

            if (images != null && images.Count > 0)
                await SaveImagesAsync(variant.VariantID, images, isFirstMain: true);

            TempData["Success"] = "Thêm biến thể thành công!";
            return RedirectToAction(nameof(Index), new { productId = model.ProductID });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _variantRepo.GetByIdAsync(id);
            if (variant == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể.";
                return RedirectToAction("Index", "Product");
            }

            var model = new VariantFormVM
            {
                VariantID = variant.VariantID,
                ProductID = variant.ProductID,
                ProductName = variant.Product.ProductName,
                Color = variant.Color,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                IsActive = variant.IsActive,
                ExistingImages = await _imageRepo.GetByVariantAsync(id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VariantFormVM model, List<IFormFile>? images)
        {
            if (await _variantRepo.ColorExistsAsync(model.ProductID, model.Color.Trim(), model.VariantID))
                ModelState.AddModelError(nameof(model.Color), "Màu sắc này đã tồn tại cho sản phẩm.");

            var variant = await _variantRepo.GetByIdAsync(model.VariantID);
            if (variant == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể.";
                return RedirectToAction("Index", "Product");
            }

            if (!ModelState.IsValid)
            {
                model.ProductName = variant.Product.ProductName;
                model.ExistingImages = await _imageRepo.GetByVariantAsync(model.VariantID);
                return View(model);
            }

            variant.Color = model.Color.Trim();
            variant.Price = model.Price;
            variant.StockQuantity = model.StockQuantity;
            variant.IsActive = model.IsActive;

            await _variantRepo.UpdateAsync(variant);

            if (images != null && images.Count > 0)
            {
                var existing = await _imageRepo.GetByVariantAsync(model.VariantID);
                await SaveImagesAsync(model.VariantID, images, isFirstMain: existing.Count == 0);
            }

            TempData["Success"] = "Cập nhật biến thể thành công!";
            return RedirectToAction(nameof(Index), new { productId = model.ProductID });
        }

        private async Task SaveImagesAsync(int variantId, List<IFormFile> images, bool isFirstMain)
        {
            var existingCount = (await _imageRepo.GetByVariantAsync(variantId)).Count;
            var newImages = new List<ProductImage>();
            var order = existingCount;

            foreach (var file in images)
            {
                if (!FileHelper.IsValidImage(file, out var error))
                {
                    TempData["Error"] = error;
                    continue;
                }

                var url = await FileHelper.SaveImageAsync(file, _env.WebRootPath);
                order++;

                newImages.Add(new ProductImage
                {
                    VariantID = variantId,
                    ImageUrl = url,
                    IsMain = isFirstMain && order == 1,
                    DisplayOrder = order
                });
            }

            if (newImages.Count > 0)
                await _imageRepo.AddRangeAsync(newImages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int variantId)
        {
            var image = await _imageRepo.GetByIdAsync(imageId);

            if (image != null)
            {
                FileHelper.DeleteImage(image.ImageUrl, _env.WebRootPath);
                await _imageRepo.DeleteAsync(imageId);

                var remaining = await _imageRepo.GetByVariantAsync(variantId);
                if (image.IsMain && remaining.Count > 0)
                    await _imageRepo.SetMainAsync(variantId, remaining.First().ImageID);

                TempData["Success"] = "Đã xóa ảnh.";
            }

            return RedirectToAction(nameof(Edit), new { id = variantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int imageId, int variantId)
        {
            await _imageRepo.SetMainAsync(variantId, imageId);
            TempData["Success"] = "Đã đặt làm ảnh chính.";
            return RedirectToAction(nameof(Edit), new { id = variantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            var images = await _imageRepo.GetByVariantAsync(id);
            foreach (var image in images)
                FileHelper.DeleteImage(image.ImageUrl, _env.WebRootPath);

            try
            {
                await _variantRepo.DeleteAsync(id);
                TempData["Success"] = "Xóa biến thể thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa biến thể vì đã có đơn hàng liên quan.";
            }

            return RedirectToAction(nameof(Index), new { productId });
        }
    }
}