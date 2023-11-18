using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Authorization")]
    [AllowAnonymous]
    public class RoleClaimController : ControllerBase
    {
        private readonly IRoleClaimService _userClaimsService;

        public RoleClaimController(IRoleClaimService userClaimsService)
        {
            _userClaimsService = userClaimsService;
        }


        [HttpGet("get-claims", Name = "get-claims")]
        [SwaggerOperation(Summary = "claims of selected role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns claim types and values", Type = typeof(RoleClaimResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = " ", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetClaims(string role)
        {
            var result = await _userClaimsService.GetUserClaims(role);
            return Ok(result);
        }



        [HttpPost("add-claim", Name = "add-claim")]
        [SwaggerOperation(Summary = "add claim to role")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns claim type and value", Type = typeof(RoleClaimResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to add claim", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddClaim([FromBody] RoleClaimRequest request)
        {
            var result = await _userClaimsService.AddClaim(request);
            return Ok(result);
        }



        [HttpDelete("delete-claim", Name = "delete-claim")]
        [SwaggerOperation(Summary = "deletes claims")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Success")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to delete claim", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteClaim(string claimValue, string role)
        {
            await _userClaimsService.RemoveUserClaims(claimValue, role);
            return Ok();
        }



        [HttpPut("edit-claim", Name = "edit-claim")]
        [SwaggerOperation(Summary = "edit claim")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Success", Type = typeof(RoleClaimResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Failed to Edit claim", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> EditClaim([FromBody] UpdateRoleClaimsDto request)
        {
            var response = await _userClaimsService.UpdateRoleClaims(request);
            return Ok(response);
        }
    }
}
