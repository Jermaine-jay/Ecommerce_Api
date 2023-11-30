using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Entities;


namespace Ecommerce_Api.Attribute
{
    public class AuthHandler : AuthorizationHandler<AuthRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationRoleClaim> _roleClaimsManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<ApplicationRole> _roleRepository;


        public AuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<ApplicationUser>();
            _roleRepository = _unitOfWork.GetRepository<ApplicationRole>();
            _roleClaimsManager = _unitOfWork.GetRepository<ApplicationRoleClaim>();
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
        {
            List<ApplicationRoleClaim> _roleClaims = new();

            var isAuthenticatedUser = context.User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier);
            if (!isAuthenticatedUser)
            {
                throw new UnauthorizedAccessException("User Not Authenticated");
            }

            var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("UnAuthenticated");
            }

            var endpoint = _httpContextAccessor.HttpContext.GetEndpoint();
            var routeName = endpoint?.Metadata.GetMetadata<EndpointNameMetadata>().EndpointName;


            var user = await _userRepository.GetSingleByAsync(u => u.UserName == userId || u.Id.ToString() == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                var userRole = await _roleRepository.GetSingleByAsync(x => x.Name == role);
                var roleClaims = await _roleClaimsManager.GetByAsync(x => x.RoleId == userRole.Id);

                if (roleClaims.Any())
                {
                    foreach (var item in roleClaims)
                    {
                        _roleClaims.Add(item);
                    }
                }
            }


            if (!_roleClaims.Any(claim => claim.ClaimType == routeName))
            {
                context.Fail();
                throw new UnauthorizedAccessException("Unauthorized");
            }

            context.Succeed(requirement);
        
        }
    }
}
