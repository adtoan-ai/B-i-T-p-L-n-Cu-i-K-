using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.ViewModels
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [StringLength(100)]
        [Display(Name = "Tên người nhận")]
        public string ReceiverName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [Display(Name = "Số điện thoại")]
        public string ReceiverPhone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD";

        public CartVM Cart { get; set; } = new();
    }
}