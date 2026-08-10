using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandRepository _brandRepo;

        public BrandController(IBrandRepository brandRepo)
        {
            _brandRepo = brandRepo;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _brandRepo.GetAllAsync();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.BrandName = model.BrandName.Trim();
            model.Description = model.Description?.Trim();

            await _brandRepo.AddAsync(model);
            TempData["Success"] = "Thêm thương hiệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandRepo.GetByIdAsync(id);
            if (brand == null)
            {
                TempData["Error"] = "Không tìm thấy thương hiệu.";
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Brand model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var brand = await _brandRepo.GetByIdAsync(model.BrandID);
            if (brand == null)
            {
                TempData["Error"] = "Không tìm thấy thương hiệu.";
                return RedirectToAction(nameof(Index));
            }

            brand.BrandName = model.BrandName.Trim();
            brand.Description = model.Description?.Trim();

            await _brandRepo.UpdateAsync(brand);
            TempData["Success"] = "Cập nhật thương hiệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _brandRepo.HasProductsAsync(id))
            {
                TempData["Error"] = "Không thể xóa thương hiệu đang có sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            await _brandRepo.DeleteAsync(id);
            TempData["Success"] = "Xóa thương hiệu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}