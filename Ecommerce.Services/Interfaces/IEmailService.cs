using Ecommerce.Models.Entities;

namespace Ecommerce.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);
        Task<string> ResetPasswordMail(ApplicationUser user);
    }
}
