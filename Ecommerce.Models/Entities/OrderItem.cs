using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}
