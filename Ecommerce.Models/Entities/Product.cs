using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Product : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductVariation>? ProductVariation { get; set; }
        
    }
}
