using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models.Entities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
    }
}
