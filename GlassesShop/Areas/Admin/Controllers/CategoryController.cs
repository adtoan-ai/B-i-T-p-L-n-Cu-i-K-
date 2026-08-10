using GlassesShop.Models.Entities;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CategoryName = model.CategoryName.Trim();
            model.Description = model.Description?.Trim();

            await _categoryRepo.AddAsync(model);
            TempData["Success"] = "Thêm danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = await _categoryRepo.GetByIdAsync(model.CategoryID);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }

            category.CategoryName = model.CategoryName.Trim();
            category.Description = model.Description?.Trim();

            await _categoryRepo.UpdateAsync(category);
            TempData["Success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _categoryRepo.HasProductsAsync(id))
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm.";
                return RedirectToAction(nameof(Index));
            }

            await _categoryRepo.DeleteAsync(id);
            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}