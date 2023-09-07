using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.Services.Implementations
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthServices(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<object> GoogleAuth(AuthenticateResult authenticateResult)
        {

           /* var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _goolgeSettings.GetSection("clientId").Value }
            };*/

            var googleUser = authenticateResult.Principal;
            var googleUserId = googleUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = googleUser.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = googleUser.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = googleUser.FindFirst(ClaimTypes.Surname)?.Value;
            var phoneNumber = googleUser.FindFirst(ClaimTypes.MobilePhone)?.Value;

            // Check if the user already exists in your database
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                // If the user doesn't exist, create a new user in your database
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber
                };

                newUser.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(newUser);
                return result;
            }
            /* var loginInfo = new UserLoginInfo()

             await _userManager.AddLoginAsync(existingUser, googleUser);
             // If the user already exists, sign them in
             await _signInManager.SignInAsync(existingUser, isPersistent: false);

                 // Redirect to a suitable page after successful login*/
            return false;
        }
    }
}
