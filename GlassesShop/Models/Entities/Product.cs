using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string ProductName { get; set; } = null!;

        public int CategoryID { get; set; }

        public int BrandID { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kiểu dáng")]
        [StringLength(50)]
        public string Style { get; set; } = null!;

        [StringLength(100)]
        public string? Material { get; set; }

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}