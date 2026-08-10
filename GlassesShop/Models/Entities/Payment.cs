using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentMethod { get; set; } = "COD";

        [Required]
        [StringLength(30)]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? TransactionCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PaidAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}