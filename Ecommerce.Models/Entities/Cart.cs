using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Cart : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<CartItem> CartItems { get; set; }

    }
}
