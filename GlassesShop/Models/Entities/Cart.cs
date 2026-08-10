using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassesShop.Models.Entities
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        public int CustomerID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Customer Customer { get; set; } = null!;
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}