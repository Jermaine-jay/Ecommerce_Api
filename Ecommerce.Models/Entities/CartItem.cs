using Ecommerce.Models.Enums;

namespace Ecommerce.Models.Entities
{
    public class CartItem : BaseEntity
    {
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public Guid? CartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Colour Colour { get; set; }
    }
}
