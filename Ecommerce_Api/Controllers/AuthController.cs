using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;



namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthServices _authServices;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(IAuthServices authServices, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _authServices = authServices;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }



        [AllowAnonymous]
        [HttpPost("register", Name = "register")]
        [SwaggerOperation(Summary = "register user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user", Type = typeof(ApplicationUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "user already exist with email", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "failed to create user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            var response = await _authServices.RegisterUser(request);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("LoginWithFacebook", Name = "LoginWithFacebook")]
        [SwaggerOperation(Summary = "Authenticates a user with facebook")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user token", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "unauthorized user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid external authentication", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> FacebookAuth([FromBody] string accessToken)
        {
            var result = await _authServices.FaceBookAuth(accessToken);
            if (!result.IsExisting)
                return RedirectToAction(nameof(ChangePassword));

            return Ok(result);
        }



        [AllowAnonymous]
        [HttpPost("LoginWithGoogle")]
        [SwaggerOperation(Summary = "Authenticates a user with google")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user token", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "No info", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid external authentication", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GoogleAuth([FromBody] string accessToken)
        {
            var response = await _authServices.GoogleAuth(accessToken);
            if (!response.IsExisting)
                return RedirectToAction(nameof(ChangePassword));

            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("change-password")]
        [SwaggerOperation(Summary = "Change user password")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "user token", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "unauthorized user", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid external authentication", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _authServices.ChangePassword(userId, request);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("login", Name = "login")]
        [SwaggerOperation(Summary = "Authenticates user")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "returns jwt token", Type = typeof(AuthenticationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid username or password", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest loginRequest)
        {
            var response = await _authServices.UserLogin(loginRequest);
            return Ok(response);
        }



        [Authorize]
        [HttpPost("test", Name = "test")]
        [SwaggerOperation(Summary = "register user")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Test()
        {
            var response = await _authServices.ChangeSocialDetails();
            return Ok(response);
        }
    }
}
