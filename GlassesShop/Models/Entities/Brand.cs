using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.Entities
{
    public class Brand
    {
        [Key]
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [StringLength(100)]
        public string BrandName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}