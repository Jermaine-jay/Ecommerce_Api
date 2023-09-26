using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? UserName { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal Total { get; set; }
        public bool Recieved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
