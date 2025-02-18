namespace Ecommerce.Models.Entities
{
    public class ShippingAddress : BaseEntity
    {
        public string Id { get; set; }
        public string? HomeNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }
        public string? OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
