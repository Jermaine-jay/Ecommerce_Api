using Ecommerce.Models.Enums;
using System.Text.Json.Serialization;

namespace Ecommerce.Models.Entities
{
    public class OrderItem : BaseEntity
    {
        public string? ProductName { get; set; }
        public Colour Colour { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [JsonIgnore]
        public Guid? OrderId { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
