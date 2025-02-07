using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.Otp;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Infrastructure;
using Ecommerce.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Ecommerce.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IOtpService _otpService;
        private readonly AppConstants _appConstants;
        private readonly IConfiguration _configuration;
        private readonly IGenerateEmailPage _generateEmailPage;
        private readonly EmailSenderOptions _emailSenderOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AppConstants appConstants,
             IOtpService otpService, IGenerateEmailPage generateEmailPage, EmailSenderOptions emailSenderOptions)
        {
            _otpService = otpService;
            _appConstants = appConstants;
            _configuration = configuration;
            _generateEmailPage = generateEmailPage;
            _emailSenderOptions = emailSenderOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("TaskManager", _emailSenderOptions.Username));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_emailSenderOptions.SmtpServer, _emailSenderOptions.Port, true);
                client.Authenticate(_emailSenderOptions.Email, _emailSenderOptions.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }

        public async Task<string> ResetPasswordMail(ApplicationUser user)
        {
            string? validToken = await _otpService.GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.PasswordReset);
            string appUrl = $"{_appConstants.AppUrl}api/Auth/reset-password?Token={validToken}";

            string page = _generateEmailPage.PasswordResetPage(appUrl);
            await SendEmailAsync(user.Email, "Reset Password", page);
            return validToken;
        }

    }
}
