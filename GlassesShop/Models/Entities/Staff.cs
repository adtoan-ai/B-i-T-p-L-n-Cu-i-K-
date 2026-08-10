using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.Entities
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        public int AccountID { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [StringLength(15)]
        public string NumberPhone { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [StringLength(300)]
        public string? Address { get; set; }

        public Account Account { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}