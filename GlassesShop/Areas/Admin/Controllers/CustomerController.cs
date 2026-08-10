using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IOrderRepository _orderRepo;

        public CustomerController(ICustomerRepository customerRepo, IAccountRepository accountRepo, IOrderRepository orderRepo)
        {
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            var customers = await _customerRepo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                customers = customers.Where(c => c.FullName.ToLower().Contains(k)
                                              || c.NumberPhone.Contains(k)
                                              || c.Email.ToLower().Contains(k)).ToList();
            }

            ViewBag.Keyword = keyword;

            var model = new List<AdminCustomerVM>();

            foreach (var c in customers)
            {
                model.Add(new AdminCustomerVM
                {
                    CustomerID = c.CustomerID,
                    AccountID = c.AccountID,
                    Username = c.Account.Username,
                    FullName = c.FullName,
                    Gender = c.Gender,
                    NumberPhone = c.NumberPhone,
                    Email = c.Email,
                    Address = c.Address,
                    IsLocked = c.Account.IsLocked,
                    CreatedAt = c.Account.CreatedAt,
                    OrderCount = await _customerRepo.CountOrdersAsync(c.CustomerID)
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            var orders = await _orderRepo.GetByCustomerAsync(id);
            ViewBag.Orders = orders.Select(GlassesShop.Services.OrderService.MapToListItem).ToList();

            return View(new AdminCustomerVM
            {
                CustomerID = customer.CustomerID,
                AccountID = customer.AccountID,
                Username = customer.Account.Username,
                FullName = customer.FullName,
                Gender = customer.Gender,
                NumberPhone = customer.NumberPhone,
                Email = customer.Email,
                Address = customer.Address,
                IsLocked = customer.Account.IsLocked,
                CreatedAt = customer.Account.CreatedAt,
                OrderCount = orders.Count
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var account = await _customerRepo.GetAccountByCustomerIdAsync(id);

            if (account == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản khách hàng.";
                return RedirectToAction(nameof(Index));
            }

            account.IsLocked = !account.IsLocked;
            await _accountRepo.UpdateAsync(account);

            TempData["Success"] = account.IsLocked
                ? $"Đã khóa tài khoản \"{account.Username}\"."
                : $"Đã mở khóa tài khoản \"{account.Username}\".";

            return RedirectToAction(nameof(Index));
        }
    }
}