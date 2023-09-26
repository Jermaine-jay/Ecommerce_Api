using System.Security.Claims;

namespace Ecommerce_Api.Extensions
{
    public static class CustomClaimsPrincipal
    {
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
        {
            return user.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
        }
    }
}
