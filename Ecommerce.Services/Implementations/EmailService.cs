using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.Otp;
using Ecommerce.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Ecommerce.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOtpService _otpService;
        private readonly IGenerateEmailPage _generateEmailPage;

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
             IOtpService otpService, IGenerateEmailPage generateEmailPage)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _otpService = otpService;
            _generateEmailPage = generateEmailPage;
        }


        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TaskManager", _configuration["EmailSenderOptions:Username"]));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_configuration["EmailSenderOptions:SmtpServer"], int.Parse(_configuration["EmailSenderOptions:Port"]), true);
                client.Authenticate(_configuration["EmailSenderOptions:Email"], _configuration["EmailSenderOptions:Password"]);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }


        public async Task<string> ResetPasswordMail(ApplicationUser user)
        {
            var validToken = await _otpService.GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.PasswordReset);
            string appUrl = $"{_configuration["AppUrl:Url"]}api/Auth/reset-password?Token={validToken}";

            var page = _generateEmailPage.PasswordResetPage(appUrl);
            await SendEmailAsync(user.Email, "Reset Password", page);
            return validToken;
        }

    }
}
