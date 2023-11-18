namespace Ecommerce.Services.Configurations.Cache.Security
{
    public interface ILoginAttempt
    {
        Task<string> LoginAttemptAsync(string userId);
        Task<AttemptDto> CheckLoginAttemptAsync(string userId);
        Task ResetLoginAttemptAsync(string userId);
    }
}
