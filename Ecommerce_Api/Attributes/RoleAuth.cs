using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Api.Attribute
{
    public class AuthRequirement : IAuthorizationRequirement
    {
        private readonly string _routeName;
    }
}
