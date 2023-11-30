using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;

namespace Ecommerce.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthenticationResponse> GoogleAuth(string credential);
        Task<AuthenticationResponse> FaceBookAuth(string credential);
        Task<object> ChangeSocialDetails();
        Task<ApplicationUser> RegisterUser(UserRegistrationRequest request);
        Task<AuthenticationResponse> UserLogin(LoginRequest request);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
        Task<SuccessResponse> ResetPassword(ResetPasswordRequest request);
        Task<ResetPasswordResponse> ForgotPassword(ForgotPasswordRequest request);
    }
}
