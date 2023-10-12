namespace Ecommerce.Services.Configurations.Cache.Otp
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId, OtpOperation operation);
        Task<bool> VerifyOtpAsync(string userId, string otp, OtpOperation operation);
        Task<string> GenerateUniqueOtpAsync(string userId, OtpOperation operation);
        Task<bool> VerifyUniqueOtpAsync(string userId, string otp, OtpOperation operation);
    }
}
