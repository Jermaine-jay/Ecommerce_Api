namespace Ecommerce.Models.Entities
{
    public class ProductCategories
    {
        public string CategoryId { get; set; }
        public string ProductId { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }

    }
}
