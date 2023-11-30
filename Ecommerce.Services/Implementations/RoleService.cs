using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepository<ApplicationRole> _roleRepo;
        private readonly IRepository<ApplicationRoleClaim> _roleClaimRepo;
        private readonly IUnitOfWork _unitOfWork;


        public RoleService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            ;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
            _roleClaimRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
        }


        public async Task<AddUserToRoleResponse> AddUserToRole(AddUserToRoleRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.Email.Trim().ToLower());
            if (user == null)
                throw new InvalidOperationException("Project does not exist");

            var role = await _roleManager.FindByNameAsync(request.Role.ToLower().Trim());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            await _userManager.AddToRoleAsync(user, role.Name);
            return new AddUserToRoleResponse
            {
                Message = $"{user} added to {role.Name}",
                UserName = user.FirstName,
            };
        }


        public async Task<SuccessResponse> CreateRoleAync(RoleDto request)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(request.Name.Trim().ToLower());
            if (role != null)
                throw new InvalidOperationException("Project does not exist");

            var applicationRole = new ApplicationRole
            {
                Name = request.Name,
            };


            await _roleManager.CreateAsync(applicationRole);
            return new SuccessResponse
            {

                Success = true,
                Data = applicationRole
            };
        }


        public async Task<SuccessResponse> DeleteRole(string name)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(name.Trim().ToLower());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            await _roleManager.DeleteAsync(role);
            return new SuccessResponse
            {

                Success = true
            };
        }


        public async Task<SuccessResponse> EditRole(string id, string Name)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(id.Trim().ToLower());
            if (role == null)
                throw new InvalidOperationException("Project does not exist");

            role.Name = Name;
            await _roleManager.UpdateAsync(role);

            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<SuccessResponse> RemoveUserFromRole(AddUserToRoleRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.Email.Trim().ToLower());
            if (user == null)
                throw new InvalidOperationException("Project does not exist");


            var myRoles = _roleManager.Roles.Select(x => x.Name);
            if (!myRoles.Contains(request.Role))
            {
                return new SuccessResponse
                {
                    Success = false
                };
            }

            var userIsInRole = await _userManager.RemoveFromRoleAsync(user, request.Role);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<IEnumerable<string>> GetUserRoles(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new InvalidOperationException("Project does not exist");

            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            if (!userRoles.Any())
            {
                return userRoles;
            }

            return userRoles;
        }


        public async Task<IEnumerable<RoleResponse>> GetAllRoles()
        {
            var roleQueryable = await _roleRepo.GetAllAsync(include: u => u.Include(x => x.RoleClaims));
            roleQueryable = roleQueryable.Where(r => r.Active);

            var roleResponseQueryable = roleQueryable.Select(s => new RoleResponse
            {
                Name = s.Name,
                Claims = s.RoleClaims.Where(r => r.ClaimValue.ToLower() is not null && r.Active),
                Active = s.Active
            });

            return roleResponseQueryable;
        }

    }
}
