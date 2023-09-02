namespace Ecommerce.Models.Entities
{
    public class Cart : BaseEntity
    {
        public Guid ApplicationUseId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
