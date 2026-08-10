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
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IAccountRepository _accountRepo;

        public StaffController(IStaffRepository staffRepo, IAccountRepository accountRepo)
        {
            _staffRepo = staffRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var staffs = await _staffRepo.GetAllAsync();
            return View(staffs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new StaffFormVM { Role = "Staff" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffFormVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError(nameof(model.Password), "Vui lòng nhập mật khẩu");

            if (await _accountRepo.UsernameExistsAsync(model.Username))
                ModelState.AddModelError(nameof(model.Username), "Tên đăng nhập đã tồn tại");

            if (await _staffRepo.EmailExistsAsync(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng");

            if (model.Role != "Staff" && model.Role != "Admin")
                ModelState.AddModelError(nameof(model.Role), "Vai trò không hợp lệ");

            if (!ModelState.IsValid)
                return View(model);

            var account = new Account
            {
                Username = model.Username.Trim(),
                PasswordHash = PasswordHelper.Hash(model.Password!),
                Role = model.Role,
                IsLocked = false,
                CreatedAt = DateTime.Now
            };

            var staff = new GlassesShop.Models.Entities.Staff
            {
                FullName = model.FullName.Trim(),
                NumberPhone = model.NumberPhone.Trim(),
                Email = model.Email.Trim(),
                Address = model.Address?.Trim()
            };

            await _staffRepo.CreateStaffAccountAsync(account, staff);

            TempData["Success"] = "Thêm tài khoản nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            var model = new StaffFormVM
            {
                StaffID = staff.StaffID,
                AccountID = staff.AccountID,
                Username = staff.Account.Username,
                FullName = staff.FullName,
                NumberPhone = staff.NumberPhone,
                Email = staff.Email,
                Address = staff.Address,
                Role = staff.Account.Role,
                IsLocked = staff.Account.IsLocked
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StaffFormVM model)
        {
            if (await _staffRepo.EmailExistsAsync(model.Email, model.StaffID))
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng");

            if (model.Role != "Staff" && model.Role != "Admin")
                ModelState.AddModelError(nameof(model.Role), "Vai trò không hợp lệ");

            var staff = await _staffRepo.GetByIdAsync(model.StaffID);
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Username = staff.Account.Username;
                return View(model);
            }

            staff.FullName = model.FullName.Trim();
            staff.NumberPhone = model.NumberPhone.Trim();
            staff.Email = model.Email.Trim();
            staff.Address = model.Address?.Trim();

            await _staffRepo.UpdateAsync(staff);

            if (staff.Account.Role != model.Role && staff.AccountID != User.GetAccountId())
            {
                staff.Account.Role = model.Role;
                await _accountRepo.UpdateAsync(staff.Account);
            }

            TempData["Success"] = "Cập nhật nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ResetPasswordVM
            {
                AccountID = staff.AccountID,
                Username = staff.Account.Username,
                FullName = staff.FullName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _accountRepo.GetByIdAsync(model.AccountID);
            if (account == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            account.PasswordHash = PasswordHelper.Hash(model.NewPassword);
            await _accountRepo.UpdateAsync(account);

            TempData["Success"] = $"Đã đổi mật khẩu cho tài khoản \"{account.Username}\".";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            if (staff.AccountID == User.GetAccountId())
            {
                TempData["Error"] = "Không thể khóa tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }

            staff.Account.IsLocked = !staff.Account.IsLocked;
            await _accountRepo.UpdateAsync(staff.Account);

            TempData["Success"] = staff.Account.IsLocked
                ? $"Đã khóa tài khoản \"{staff.Account.Username}\"."
                : $"Đã mở khóa tài khoản \"{staff.Account.Username}\".";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            if (staff.AccountID == User.GetAccountId())
            {
                TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }

            if (await _staffRepo.HasOrdersAsync(id))
            {
                TempData["Error"] = "Không thể xóa nhân viên đang phụ trách đơn hàng. Hãy khóa tài khoản thay thế.";
                return RedirectToAction(nameof(Index));
            }

            await _staffRepo.DeleteAsync(id);
            TempData["Success"] = "Xóa tài khoản nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}