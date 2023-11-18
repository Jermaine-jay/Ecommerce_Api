namespace Ecommerce.Models.Dtos.Requests
{

    public class RoleClaimRequest
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }

    public class RoleClaimResponse
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
    }

    public class UpdateRoleClaimsDto
    {
        public string Role { get; set; }
        public string ClaimType { get; set; }
        public string NewClaim { get; set; }
    }

}
