using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4 đến 50 ký tự")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Customer";

        public bool IsLocked { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }
    }
}