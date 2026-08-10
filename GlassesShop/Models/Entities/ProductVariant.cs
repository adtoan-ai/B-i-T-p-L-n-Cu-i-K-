using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class ProductVariant
    {
        [Key]
        public int VariantID { get; set; }

        public int ProductID { get; set; }

        [Required(ErrorMessage = "Màu sắc không được để trống")]
        [StringLength(50)]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(1, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public Product Product { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}