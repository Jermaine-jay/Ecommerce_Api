using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Entities
{
    public class ProductImage
    {
        [Key]
        public string? PublicId { get; set; } 
        public string? Format { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public long? Bytes { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
        public string? SecureUrl { get; set; }
        public virtual Guid? ProductVariationId { get; set; }
        public virtual ProductVariation? ProductVariation { get; set; }
    }
}
