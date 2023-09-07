using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public override string? PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
