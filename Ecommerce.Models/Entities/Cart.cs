using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Cart : BaseEntity
    {
        public Guid? UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
    }
}
