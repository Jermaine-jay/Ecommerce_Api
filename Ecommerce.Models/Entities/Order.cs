using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? UserName { get; set; }
        public decimal Total { get; set; }
        public bool Recieved { get; set; }
        public bool Paid { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public virtual ShippingAddress? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
