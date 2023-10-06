using Ecommerce.Models.Dtos.Common;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.Implementations
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtAuthenticator _jwtAuthenticator;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;


        public AuthServices(UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<ApplicationRole> roleManager, IJwtAuthenticator jwtAuthenticator, HttpClient httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtAuthenticator = jwtAuthenticator;
            _httpClient = httpClient;
        }

        public async Task<object> ChangeSocialDetails()
        {
            var input = "AUTHORIZATION WORKS";
            return input;
        }


        public async Task<AuthenticationResponse> GoogleAuth(string credential)
        {

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

            if (payload == null)
                throw new InvalidOperationException($"Invalid External Authentication.");

            var info = new UserLoginInfo("GOOGLE", payload.Name, "GOOGLE");
            if (info == null)
                throw new InvalidOperationException($"NO INFO");           

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                var newuser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Active = true,
                    UserType = UserType.User
                };
                newuser.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(newuser);
                if (!result.Succeeded)
                {
                    var message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                    throw new InvalidOperationException(message);
                }

                var role = UserType.User.GetStringValue();
                bool roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    ApplicationRole newRole = new ApplicationRole { Name = role };
                    await _roleManager.CreateAsync(newRole);
                }


                await _userManager.AddToRoleAsync(newuser, role);
                await _userManager.AddLoginAsync(newuser, info);

                var jwttoken = await _jwtAuthenticator.GenerateJwtToken(newuser);
                var fullname = $"{newuser.LastName} {newuser.FirstName}";
                return new AuthenticationResponse
                {
                    JwtToken = jwttoken,
                    UserType = newuser.UserType.GetStringValue(),
                    FullName = fullname,
                    TwoFactor = false,
                    IsExisting = false,
                };
            }


            var existuser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existuser == null)
                throw new InvalidOperationException($"User Does Not exist");

            var jwtToken = await _jwtAuthenticator.GenerateJwtToken(existuser);
            var newUserFullname = $"{existuser.LastName} {existuser.FirstName}";
            return new AuthenticationResponse
            {
                JwtToken = jwtToken,
                UserType = existuser.UserType.GetStringValue(),
                FullName = newUserFullname,
                TwoFactor = false,
                IsExisting = true
                
            };
        }


        public async Task<AuthenticationResponse> FaceBookAuth(string credential)
        {
            var debugTokenResponse = await _httpClient.GetAsync("https://graph.facebook.com/debug_token?input_token=" + credential + $"&access_token={_configuration["Authentication:Facebook:AppId"]}|{_configuration["Authentication:Facebook:AppSecret"]}");

            var stringThing = await debugTokenResponse.Content.ReadAsStringAsync();
            var userOBJK = JsonConvert.DeserializeObject<FBUser>(stringThing);

            if (userOBJK.Data.IsValid == false)
                throw new InvalidOperationException("UnAuthorized user");

            HttpResponseMessage meResponse = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token=" + credential);
            var userContent = await meResponse.Content.ReadAsStringAsync();

            var payload = JsonConvert.DeserializeObject<FBUserInfo>(userContent);
            if (payload == null)
                throw new InvalidOperationException($"Invalid External Authentication.");

            var info = new UserLoginInfo("Facebook", payload.Id, "Facebook");
            if (info == null)
                throw new InvalidOperationException($"NO INFO");
            
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                var newuser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.FirstName,
                    LastName = payload.LastName,
                    Active = true,
                    UserType = UserType.User
                };
                newuser.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(newuser);
                if (!result.Succeeded)
                {
                    var message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                    throw new InvalidOperationException(message);
                }

                var role = UserType.User.GetStringValue();
                bool roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    ApplicationRole newRole = new ApplicationRole { Name = role };
                    await _roleManager.CreateAsync(newRole);
                }


                await _userManager.AddToRoleAsync(newuser, role);
                await _userManager.AddLoginAsync(newuser, info);

                var jwttoken = await _jwtAuthenticator.GenerateJwtToken(newuser);
                var newUserFullname = $"{newuser.LastName} {newuser.FirstName}";
                return new AuthenticationResponse
                {
                    JwtToken = jwttoken,
                    UserType = newuser.UserType.GetStringValue(),
                    FullName = newUserFullname,
                    TwoFactor = false,
                    IsExisting = false,
                    
                };
            }

            var existuser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existuser == null)
                throw new InvalidOperationException($"User Does Not exist");

            var jwtToken = await _jwtAuthenticator.GenerateJwtToken(user);
            var fullname = $"{user.LastName} {user.FirstName}";
            return new AuthenticationResponse
            {
                JwtToken = jwtToken,
                UserType = user.UserType.GetStringValue(),
                FullName = fullname,
                TwoFactor = false,
                IsExisting = true
                
            };
        }


        public async Task<ApplicationUser> RegisterUser(UserRegistrationRequest request)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"User already exists with Email {request.Email}");


            var emailExist = await _userManager.FindByNameAsync(request.Email);
            if (emailExist != null)
                throw new InvalidOperationException($"User already exists");


            ApplicationUser user = new()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.Firstname,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Active = true,
                UserType = UserType.User,
                EmailConfirmed = true,
            };


            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                throw new InvalidOperationException(message);
            }


            var role = UserType.User.GetStringValue();
            bool roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                ApplicationRole newRole = new ApplicationRole { Name = role };
                await _roleManager.CreateAsync(newRole);
            }

            await _userManager.AddToRoleAsync(user, role);

            return user;
        }


        public async Task<AuthenticationResponse> UserLogin(LoginRequest request)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email.ToLower().Trim());
            if (user == null)
                throw new InvalidOperationException("Invalid username or password");

            if (!user.Active)
                throw new InvalidOperationException("Account is not active");

            if (!user.EmailConfirmed)
                throw new InvalidOperationException("User Not Found");

            if (user.LockoutEnd != null)
                throw new InvalidOperationException($"User Suspended. Time Left {user.LockoutEnd - DateTimeOffset.UtcNow}");

            bool result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                user.AccessFailedCount++;
                throw new InvalidOperationException("Invalid username or password");
            }

            if (user.AccessFailedCount == 5)
            {
                DateTimeOffset lockoutEnd = DateTimeOffset.UtcNow.AddSeconds(300);
                user.LockoutEnd = lockoutEnd;
                await _userManager.ResetAccessFailedCountAsync(user);
                throw new InvalidOperationException($"Account locked, Time Left {user.LockoutEnd - DateTimeOffset.UtcNow}");
            }

            JwtToken userToken = await _jwtAuthenticator.GenerateJwtToken(user);
            string? userType = user.UserType.GetStringValue();

            string fullName = $"{user.LastName} {user.FirstName}";
            return new AuthenticationResponse
            {
                JwtToken = userToken,
                UserType = userType.Normalize(),
                FullName = fullName,
                TwoFactor = false,
                IsExisting = true,
            };

        }


        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");


            await _userManager.ChangePasswordAsync(user, request.NewPassword, request.CurrentPassword);
            return new SuccessResponse
            {
                Success = true,
            };
        }
    }


    public class SuccessResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
    }

    public class ChangePasswordRequest
    {

        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }


    public class LoginRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
