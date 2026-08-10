using System.ComponentModel.DataAnnotations;

namespace GlassesShop.Models.Entities
{
    public class CartDetail
    {
        [Key]
        public int CartDetailID { get; set; }

        public int CartID { get; set; }

        public int VariantID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;

        public Cart Cart { get; set; } = null!;
        public ProductVariant Variant { get; set; } = null!;
    }
}