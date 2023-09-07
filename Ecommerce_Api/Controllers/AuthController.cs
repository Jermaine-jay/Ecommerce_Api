using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Ecommerce_Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(IAuthServices authServices, SignInManager<ApplicationUser> signInManager)
        {
            _authServices = authServices;
            _signInManager = signInManager;
        }


        [AllowAnonymous]
        [HttpPost("signin-google")]
        [SwaggerOperation(Summary = "Login with Google")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleAuth", "Auth") 
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }



        [HttpPost("googleAuth", Name = "googleAuth")]
        /*[SwaggerOperation(Summary = "Creates user")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]*/
        public async Task<IActionResult> GoogleAuth()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("GoogleLogin");
            }

            var result = await _authServices.GoogleAuth(authenticateResult);
            return Ok(result);
        }


        [Authorize]
        [HttpGet("LoginUser", Name = "LoginUser")]
        [SwaggerOperation(Summary = "Login A user")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser()
        {
            return Ok("WELCOME AMIGO");
        }
    }
}
