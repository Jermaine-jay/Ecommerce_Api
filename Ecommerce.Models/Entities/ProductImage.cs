using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Entities
{
    public class ProductImage
    {
        [Key]
        public string PublicId { get; set; } 
        public string Version { get; set; }
        public string Signature { get; set; } 
        public string Width { get; set; } 
        public string Height { get; set; } 
        public string Format { get; set; } 
        public string ResourceType { get; set; }
        public string CreatedAt { get; set; }
        public string Bytes { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string SecureUrl { get; set; }
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
