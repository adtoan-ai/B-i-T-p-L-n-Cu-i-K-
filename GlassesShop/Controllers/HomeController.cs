using System.Diagnostics;
using GlassesShop.Models;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IBrandRepository _brandRepo;

        public HomeController(IProductRepository productRepo, IBrandRepository brandRepo)
        {
            _productRepo = productRepo;
            _brandRepo = brandRepo;
        }

        public async Task<IActionResult> Index()
        {
            var latest = await _productRepo.GetLatestAsync(8);

            var model = new HomeVM
            {
                LatestProducts = latest.Select(ProductController.MapToCard).ToList(),
                Brands = await _brandRepo.GetAllAsync()
            };

            return View(model);
        }

        [Route("Home/StatusCode/{code:int}")]
        public IActionResult StatusCodeHandler(int code)
        {
            if (code == 404)
                return View("NotFoundPage");

            ViewBag.StatusCode = code;
            return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}