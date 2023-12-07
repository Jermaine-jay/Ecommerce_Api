using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Authorization")]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public UserController(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _httpContextAccessor = contextAccessor;
            _userService = userService;
        }


        [HttpGet("profile", Name = "profile")]
        [SwaggerOperation(Summary = "get loggedin user account ")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user", Type = typeof(ProfileResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.Profile(userId);
            return Ok(response);
        }



        [HttpPut("change-password", Name = "change-password")]
        [SwaggerOperation(Summary = "Change user password")]
        [SwaggerResponse(StatusCodes.Status202Accepted, Description = "true", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.ChangePassword(userId, request);
            return Ok(response);
        }



        [HttpDelete("delete-account", Name = "delete-account")]
        [SwaggerOperation(Summary = "delete a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "true", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.DeleteAccount(userId);
            return Ok(response);
        }



        [HttpPut("update-account", Name = "update-account")]
        [SwaggerOperation(Summary = "update a user")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "true", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.UpdateAccount(userId, request);
            return Ok(response);
        }



        [HttpGet("get-cart", Name = "get-cart")]
        [SwaggerOperation(Summary = "get user cart")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "cartitem", Type = typeof(Cart))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetCart()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.GetCart(userId);
            return Ok(response);
        }



        [HttpPost("add-to-cart", Name = "add-to-cart")]
        [SwaggerOperation(Summary = "add item to user cart")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "user", Type = typeof(CartItemResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.AddToCart(userId, request);
            return Ok(response);
        }



        [HttpDelete("delete-from-cart", Name = "delete-from-cart")]
        [SwaggerOperation(Summary = "delete an item from user cart")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "cartitem", Type = typeof(CartItemResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User cart Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteFromCart(string cartitemId)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.DeleteFromCart("994f5f3d-f22b-407e-8016-ee7ed508da4e", cartitemId);
            return Ok(response);
        }


        [HttpDelete("delete-cartitems", Name = "delete-cartitems")]
        [SwaggerOperation(Summary = "delete all user cart items")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user", Type = typeof(CartItemResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteCartItems()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _userService.DeleteCartItems(userId);
            return Ok(response);
        }
    }
}
