using Ecommerce.Models.Enums;

namespace Ecommerce.Models.Entities
{
    public class ProductVariation : BaseEntity
    {
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public Colour Colour { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
    }
}
