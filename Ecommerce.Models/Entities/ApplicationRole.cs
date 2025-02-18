using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models.Entities
{
    public class ApplicationRole : IdentityRole<string>
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

    }
}
