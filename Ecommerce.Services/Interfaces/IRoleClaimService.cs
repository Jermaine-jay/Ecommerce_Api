using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;

namespace Ecommerce.Services.Interfaces
{
    public interface IRoleClaimService
    {
        Task<RoleClaimResponse> UpdateRoleClaims(UpdateRoleClaimsDto request);
        Task<string> RemoveUserClaims(string claimType, string role);
        Task<SuccessResponse> GetUserClaims(string? role);
        Task<RoleClaimResponse> AddClaim(RoleClaimRequest request);
    }
}
