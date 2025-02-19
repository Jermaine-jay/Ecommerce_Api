using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Order : BaseEntity
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public decimal Total { get; set; }
        public bool Received { get; set; }
        public bool Paid { get; set; }
        public string? Txnref { get; set; }
        public string? ShippingAddressId { get; set; }
        public virtual ShippingAddress? ShippingAddress { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
