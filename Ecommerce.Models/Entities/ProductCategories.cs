namespace Ecommerce.Models.Entities
{
    public class ProductCategories
    {
        public Guid CategoryId { get; set; }
        public Guid ProductId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }

    }
}
