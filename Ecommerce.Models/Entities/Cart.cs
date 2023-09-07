using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Cart : BaseEntity
    {
        public Guid ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
