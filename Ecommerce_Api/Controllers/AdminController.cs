using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Authorization")]

    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

       
        [HttpGet("all-users", Name = "all-users")]
        [SwaggerOperation(Summary = "Get All Registered Users")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Users", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _adminService.GetUsers();
            return Ok(response);
        }



        [HttpDelete("remove-user", Name = "remove-User")]
        [SwaggerOperation(Summary = "Delete a user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var response = await _adminService.DeleteUser(userId);
            return Ok(response);

        }



        [HttpGet("get-user", Name = "get-user")]
        [SwaggerOperation(Summary = "Get A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser(string userId)
        {
            var response = await _adminService.GetUser(userId);
            return Ok(response);
        }


        [HttpPost("lock-user", Name = "lock-user")]
        [SwaggerOperation(Summary = "Block A Registered User")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "successful", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LockUser([FromBody] LockUserRequest request)
        {
            var response = await _adminService.LockUser(request);
            return Ok(response);
        }


        [HttpPost("create-category", Name = "create-category")]
        [SwaggerOperation(Summary = "create categories")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "category", Type = typeof(CreateCategoryResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No category found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var response = await _adminService.CreateCategory(request);
            return Ok(response);
        }


        [HttpGet("all-categories", Name = "all-categories")]
        [SwaggerOperation(Summary = "all categories")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "users pro", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No category found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _adminService.GetAllCategories();
            return Ok(response);
        }
    }
}
