namespace Ecommerce.Models.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
        public Cart Cart { get; set; }
    }
}
