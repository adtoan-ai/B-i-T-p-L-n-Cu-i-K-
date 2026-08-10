using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.ViewModels
{
    public class StaffFormVM
    {
        public int StaffID { get; set; }
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4 đến 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ, số và dấu gạch dưới")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [Display(Name = "Số điện thoại")]
        public string NumberPhone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Staff";

        public bool IsLocked { get; set; }
        public bool IsEdit => StaffID > 0;
    }

    public class ResetPasswordVM
    {
        public int AccountID { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class AdminCustomerVM
    {
        public int CustomerID { get; set; }
        public int AccountID { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string NumberPhone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderCount { get; set; }
    }
}