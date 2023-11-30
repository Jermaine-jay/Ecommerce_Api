using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using System.Net;

namespace TaskManager.Services.Implementations
{

    public class RoleClaimService : IRoleClaimService
    {
        private readonly IRepository<ApplicationRoleClaim> _roleClaimRepo;
        private readonly IRepository<ApplicationRole> _roleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RoleClaimService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _roleClaimRepo = _unitOfWork.GetRepository<ApplicationRoleClaim>();
            _roleRepo = _unitOfWork.GetRepository<ApplicationRole>();
        }


        public async Task<RoleClaimResponse> AddClaim(RoleClaimRequest request)
        {
            var getRole = await _roleRepo.GetSingleByAsync(r => r.Name.ToLower() == request.Role.ToLower());
            if (getRole == null)
                throw new InvalidOperationException("Role does not exist");

            var checkExisting = await _roleClaimRepo.GetSingleByAsync(x => x.ClaimType == request.ClaimType && x.RoleId == getRole.Id);
            if (checkExisting != null)
                throw new InvalidOperationException("Identical claim value already exist for this role");



            var newClaim = new ApplicationRoleClaim()
            {
                RoleId = getRole.Id,
                ClaimType = request.ClaimType,
            };

            await _roleClaimRepo.AddAsync(newClaim);

            return new RoleClaimResponse
            {
                Role = getRole.Name,
                ClaimType = newClaim.ClaimType
            };
            
        }


        public async Task<SuccessResponse> GetUserClaims(string? role)
        {

            ApplicationRole getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == role);
            if (getRole == null)
                throw new InvalidOperationException("Role Does Not exist");

            IEnumerable<ApplicationRoleClaim> claims = await _roleClaimRepo.GetAllAsync();
            var result = claims.Where(x => x.RoleId == getRole.Id).Select(u => new RoleClaimResponse
            {
                Role = getRole.Name,
                ClaimType = u.ClaimType
            });

            return new SuccessResponse
            {
                Success = true,
                Data = result
            };
        }


        public async Task<string> RemoveUserClaims(string claimType, string role)
        {
            var getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == role.ToLower());
            if (getRole == null)
                throw new InvalidOperationException("Role Does Not exist");

            var claim = await _roleClaimRepo.GetSingleByAsync(x => x.ClaimType == claimType && x.RoleId == getRole.Id);
            if (claim == null)
                throw new InvalidOperationException("Claim value does not exist for this role");

            await _roleClaimRepo.DeleteAsync(claim);

            var Message = $"{claim} claim Removed From {getRole} Role";
            return Message;          

        }


        public async Task<RoleClaimResponse> UpdateRoleClaims(UpdateRoleClaimsDto request)
        {
            ApplicationRole getRole = await _roleRepo.GetSingleByAsync(x => x.Name.ToLower() == request.Role);
            if (getRole == null)
                throw new InvalidOperationException("Role does not Exist, Ensure there are no spaces in the text entered");


            IEnumerable<ApplicationRoleClaim> claims = await _roleClaimRepo.GetAllAsync();
            var result = claims.Where(x => x.ClaimType == request.ClaimType && x.RoleId == getRole.Id).FirstOrDefault();

            result.ClaimType = request.NewClaim;
            await _roleClaimRepo.UpdateAsync(result);

            return new RoleClaimResponse
            {
                ClaimType = result.ClaimType,
                Role = getRole.Name
            };
        }

    }



}
