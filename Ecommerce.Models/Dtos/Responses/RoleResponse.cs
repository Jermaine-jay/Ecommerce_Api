using Ecommerce.Models.Entities;

namespace Ecommerce.Models.Dtos.Responses
{

    public class AddUserToRoleResponse
    {

        public string? Message { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }

    public class RoleResponse
    {
        public string? Name { get; set; }
        public IEnumerable<ApplicationRoleClaim> Claims { get; set; }
        public bool Active { get; set; }

    }

}
