using GlassesShop.Helpers;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ProfileController : Controller
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAccountRepository _accountRepo;

        public ProfileController(ICustomerRepository customerRepo, IAccountRepository accountRepo)
        {
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var customer = await _customerRepo.GetByIdAsync(User.GetCustomerId());
            if (customer == null) return RedirectToAction("Index", "Home");

            var model = new ProfileVM
            {
                CustomerID = customer.CustomerID,
                Username = customer.Account.Username,
                FullName = customer.FullName,
                Gender = customer.Gender,
                NumberPhone = customer.NumberPhone,
                Email = customer.Email,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            var customerId = User.GetCustomerId();

            if (await _customerRepo.PhoneExistsAsync(model.NumberPhone, customerId))
                ModelState.AddModelError(nameof(model.NumberPhone), "Số điện thoại đã được sử dụng bởi tài khoản khác");

            if (await _customerRepo.EmailExistsAsync(model.Email, customerId))
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng bởi tài khoản khác");

            if (model.Gender != "Nam" && model.Gender != "Nữ")
                ModelState.AddModelError(nameof(model.Gender), "Giới tính không hợp lệ");

            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                model.Username = customer.Account.Username;
                return View(model);
            }

            customer.FullName = model.FullName.Trim();
            customer.Gender = model.Gender;
            customer.NumberPhone = model.NumberPhone.Trim();
            customer.Email = model.Email.Trim();
            customer.Address = model.Address?.Trim();
            customer.DateOfBirth = model.DateOfBirth;

            await _customerRepo.UpdateAsync(customer);

            TempData["Success"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _accountRepo.GetByIdAsync(User.GetAccountId());
            if (account == null) return RedirectToAction("Index", "Home");

            if (!PasswordHelper.Verify(model.CurrentPassword, account.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng");
                return View(model);
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Mật khẩu mới phải khác mật khẩu hiện tại");
                return View(model);
            }

            account.PasswordHash = PasswordHelper.Hash(model.NewPassword);
            await _accountRepo.UpdateAsync(account);

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}