using System.Security.Claims;
using GlassesShop.Helpers;
using GlassesShop.Models.Entities;
using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GlassesShop.Services;

namespace GlassesShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly CartService _cartService;

        public AccountController(IAccountRepository accountRepo, ICustomerRepository customerRepo, CartService cartService)
        {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (await _accountRepo.UsernameExistsAsync(model.Username))
                ModelState.AddModelError(nameof(model.Username), "Tên đăng nhập đã tồn tại");

            if (await _customerRepo.PhoneExistsAsync(model.NumberPhone))
                ModelState.AddModelError(nameof(model.NumberPhone), "Số điện thoại đã được sử dụng");

            if (await _customerRepo.EmailExistsAsync(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng");

            if (model.Gender != "Nam" && model.Gender != "Nữ")
                ModelState.AddModelError(nameof(model.Gender), "Giới tính không hợp lệ");

            if (!ModelState.IsValid)
                return View(model);

            var account = new Account
            {
                Username = model.Username.Trim(),
                PasswordHash = PasswordHelper.Hash(model.Password),
                Role = "Customer",
                IsLocked = false,
                CreatedAt = DateTime.Now
            };

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Gender = model.Gender,
                NumberPhone = model.NumberPhone.Trim(),
                Email = model.Email.Trim(),
                Address = model.Address?.Trim(),
                DateOfBirth = model.DateOfBirth
            };

            await _accountRepo.CreateCustomerAccountAsync(account, customer);

            TempData["Success"] = "Đăng ký tài khoản thành công! Mời bạn đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _accountRepo.GetByUsernameAsync(model.Username.Trim());

            if (account == null || !PasswordHelper.Verify(model.Password, account.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            if (account.IsLocked)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            var fullName = account.Customer?.FullName ?? account.Staff?.FullName ?? account.Username;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim(ClaimsHelper.ClaimAccountId, account.AccountID.ToString()),
                new Claim(ClaimsHelper.ClaimFullName, fullName)
            };

            if (account.Customer != null)
                claims.Add(new Claim(ClaimsHelper.ClaimCustomerId, account.Customer.CustomerID.ToString()));

            if (account.Staff != null)
                claims.Add(new Claim(ClaimsHelper.ClaimStaffId, account.Staff.StaffID.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            if (account.Customer != null)
                await _cartService.MergeGuestCartAsync(account.Customer.CustomerID);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            TempData["Success"] = $"Xin chào {fullName}!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}