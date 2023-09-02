using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole(string role) : base(role)
        {

        }

        public ApplicationRole()
        {

        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; } = true;
        public UserType Type { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

    }
}
