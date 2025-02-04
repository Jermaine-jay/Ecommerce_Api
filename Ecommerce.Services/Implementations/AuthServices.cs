using Ecommerce.Models.Dtos.Common;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Cache.Otp;
using Ecommerce.Services.Configurations.Cache.Security;
using Ecommerce.Services.Extensions;
using Ecommerce.Services.Infrastructure;
using Ecommerce.Services.Interfaces;
using Ecommerce.Services.Utilities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;


namespace Ecommerce.Services.Implementations
{
    public class AuthServices : IAuthServices
    {
        private FacebookConfig _facebookConfig;
        private GoogleConfig _googleConfig;
        private readonly HttpClient _httpClient;
        private readonly IOtpService _otpService;
        private readonly ICacheService _cacheService;
        private readonly ILoginAttempt _loginAttempt;
        private readonly IEmailService _emailService;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJwtAuthenticator _jwtAuthenticator;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthServices(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, HttpClient httpClient,
            FacebookConfig facebookConfig, GoogleConfig googleConfig, IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpClient = httpClient;
            _facebookConfig = facebookConfig;
            _googleConfig = googleConfig;
            _otpService = _serviceFactory.GetService<IOtpService>();
            _cacheService = _serviceFactory.GetService<ICacheService>();
            _loginAttempt = _serviceFactory.GetService<ILoginAttempt>();
            _emailService = _serviceFactory.GetService<IEmailService>();
            _jwtAuthenticator = _serviceFactory.GetService<IJwtAuthenticator>();
        }

        public async Task<object> ChangeSocialDetails()
        {
            var input = "AUTHORIZATION WORKS";
            return input;
        }

        public async Task<AuthenticationResponse> GoogleAuth(string credential)
        {
            if (credential == null) throw new ArgumentNullException("Token is null or invalid");

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() {_googleConfig.ClientId}
            };

            GoogleJsonWebSignature.Payload payload = 
                        await GoogleJsonWebSignature.ValidateAsync(credential, settings);

            if (payload == null)
                throw new InvalidOperationException($"Invalid External Authentication.");

            UserLoginInfo info = new UserLoginInfo("GOOGLE", payload.Name, "GOOGLE");
            if (info == null)
                throw new InvalidOperationException($"No user Info");

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                ApplicationUser newuser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Active = true,
                    UserType = UserType.User,
                };

                newuser.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(newuser);
                if (!result.Succeeded)
                {
                    var message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                    throw new InvalidOperationException(message);
                }

                Cart cart = new Cart();
                string key = $"cart:{newuser.Id}";
                await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));

                string role = UserType.User.GetStringValue();
                bool roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    ApplicationRole newRole = new ApplicationRole { Name = role };
                    await _roleManager.CreateAsync(newRole);
                }

                await _userManager.AddToRoleAsync(newuser, role);
                await _userManager.AddLoginAsync(newuser, info);

                JwtToken jwttoken = await _jwtAuthenticator.GenerateJwtToken(newuser);
                string fullname = $"{newuser.LastName} {newuser.FirstName}";
                return new AuthenticationResponse
                {
                    JwtToken = jwttoken,
                    UserType = newuser.UserType.GetStringValue(),
                    FullName = fullname,
                    TwoFactor = false,
                    IsExisting = false,
                };
            }

            ApplicationUser? existuser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existuser == null)
                throw new InvalidOperationException($"User Does Not exist");

            JwtToken jwtToken = await _jwtAuthenticator.GenerateJwtToken(existuser);
            string newUserFullname = $"{existuser.LastName} {existuser.FirstName}";
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

            string? stringThing = await debugTokenResponse.Content.ReadAsStringAsync();
            FBUser? userOBJK = JsonConvert.DeserializeObject<FBUser>(stringThing);

            if (userOBJK.Data.IsValid == false)
                throw new InvalidOperationException("UnAuthorized user");

            HttpResponseMessage meResponse = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token=" + credential);
            var userContent = await meResponse.Content.ReadAsStringAsync();

            FBUserInfo? payload = JsonConvert.DeserializeObject<FBUserInfo>(userContent);
            if (payload == null)
                throw new InvalidOperationException($"Invalid External Authentication.");

            UserLoginInfo info = new UserLoginInfo("Facebook", payload.Id, "Facebook");
            if (info == null)
                throw new InvalidOperationException($"NO INFO");

            ApplicationUser? user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                ApplicationUser newuser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.FirstName,
                    LastName = payload.LastName,
                    Active = true,
                    UserType = UserType.User,
                };

                newuser.EmailConfirmed = true;

                IdentityResult result = await _userManager.CreateAsync(newuser);
                if (!result.Succeeded)
                {
                    string message = $"Failed to create user: {(result.Errors.FirstOrDefault())?.Description}";
                    throw new InvalidOperationException(message);
                }
                var cart = new Cart();
                var key = $"cart:{newuser.Id}";
                await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));

                string role = UserType.User.GetStringValue();
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

            ApplicationUser? existuser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
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
                Id = Guid.NewGuid(),
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
                string message = $"Failed to create user: {(result.Errors.SingleOrDefault())?.Description}";
                throw new InvalidOperationException(message);
            }

            var cart = new Cart();
            var key = CacheKeySelector.UserCartCacheKey(user.Id.ToString());
            await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));

            string? role = UserType.User.GetStringValue();
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
            var maxAttempt = 5;
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email.ToLower().Trim());
            if (user == null)
                throw new InvalidOperationException("Invalid username or password");

            if (!user.Active)
                throw new InvalidOperationException("Account is not active");

            if (!user.EmailConfirmed)
                throw new InvalidOperationException("User Not Found");

            if (user.LockoutEnd != null)
                throw new InvalidOperationException($"User Suspended. Time Left {user.LockoutEnd - DateTimeOffset.UtcNow}");

            string key = await _loginAttempt.LoginAttemptAsync(user.Id.ToString());
            AttemptDto check = await _loginAttempt.CheckLoginAttemptAsync(user.Id.ToString());
            if (check.Attempts == maxAttempt)
            {
                DateTimeOffset lockoutEnd = DateTimeOffset.UtcNow.AddSeconds(300);
                user.LockoutEnd = lockoutEnd;
                await _userManager.UpdateAsync(user);
                await _loginAttempt.ResetLoginAttemptAsync(user.Id.ToString());
                throw new InvalidOperationException($"Account locked, Time Left {user.LockoutEnd - DateTimeOffset.UtcNow}");
            }

            bool result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                check.Attempts += 5;
                await _cacheService.WriteToCache(key, check, null, TimeSpan.FromDays(365));
                throw new InvalidOperationException("Invalid username or password");
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

        public async Task<ResetPasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"Invalid Email Address");

            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            bool isConfrimed = await _userManager.IsEmailConfirmedAsync(user);
            if (user == null || !isConfrimed)
                throw new InvalidOperationException($"User does not exist");

            if (user.LockoutEnd != null)
                throw new InvalidOperationException($"User Suspended. Time Left {user.LockoutEnd - DateTimeOffset.UtcNow}");


            var result = await _emailService.ResetPasswordMail(user);
            return new ResetPasswordResponse
            {
                Message = "Token sent",
                Token = result,
                Success = true
            };
        }

        public async Task<SuccessResponse> ResetPassword(ResetPasswordRequest request)
        {
            var (existingUser, operation) = await DecodeToken.DecodeVerificationToken(request.Token);

            ApplicationUser? user = await _userManager.FindByIdAsync(existingUser);
            if (user == null || !user.EmailConfirmed)
                throw new InvalidOperationException($"User does not exist");

            if (operation != OtpOperation.PasswordReset.ToString())
                throw new InvalidOperationException($"Invalid Operation");

            bool isOtpValid = await _otpService.VerifyOtpAsync(user.Id.ToString(), request.Token, OtpOperation.PasswordReset);
            if (!isOtpValid)
                throw new InvalidOperationException($"Invalid Token");

            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.NewPassword, request.ConfirmPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Could not complete operation");

            return new SuccessResponse
            {
                Success = true,
                Data = result,
            };
        }
    }

}
