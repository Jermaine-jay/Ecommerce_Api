using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? UserName { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
