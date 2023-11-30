using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Dtos.Requests
{

    public class AddUserToRoleRequest
    {
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class RoleDto
    {
        [Required(ErrorMessage = "Role Name cannot be empty"), MinLength(2), MaxLength(30)]
        public string? Name { get; set; } = null!;
    }

}
