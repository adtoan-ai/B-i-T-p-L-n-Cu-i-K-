using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.Entities
{
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }

        public int VariantID { get; set; }

        [Required]
        [StringLength(300)]
        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public ProductVariant Variant { get; set; } = null!;
    }
}