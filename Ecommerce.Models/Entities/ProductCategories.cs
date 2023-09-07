using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class ProductCategories
    {
        public Guid CategoryId { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        [ForeignKey(nameof(ProductId))]
        public  virtual Product Product { get; set; }

    }
}
