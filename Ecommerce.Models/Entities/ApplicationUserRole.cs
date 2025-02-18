using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models.Entities
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
