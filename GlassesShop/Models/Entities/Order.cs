using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public int? StaffID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tên người nhận không được để trống")]
        [StringLength(100)]
        public string ReceiverName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [StringLength(15)]
        public string ReceiverPhone { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(300)]
        public string ShippingAddress { get; set; } = null!;

        [StringLength(500)]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(30)]
        public string OrderStatus { get; set; } = "Chờ xác nhận";

        public Customer Customer { get; set; } = null!;
        public Staff? Staff { get; set; }
        public Payment? Payment { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}