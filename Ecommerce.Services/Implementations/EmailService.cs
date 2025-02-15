using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.Otp;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Infrastructure;
using Ecommerce.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;

namespace Ecommerce.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IOtpService _otpService;
        private readonly AppConstants _appConstants;
        private readonly ZeroBounceConfig _zeroBounce;
        private readonly IConfiguration _configuration;
        private readonly IServiceFactory _serviceFactory;
        private readonly IGenerateEmailPage _generateEmailPage;
        private readonly EmailSenderOptions _emailSenderOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, IServiceFactory serviceFactory)
        {
            _configuration = configuration;
            _serviceFactory = serviceFactory;
            _httpContextAccessor = httpContextAccessor;
            _otpService = _serviceFactory.GetService<IOtpService>();
            _appConstants = _serviceFactory.GetService<AppConstants>();
            _zeroBounce = _serviceFactory.GetService<ZeroBounceConfig>();
            _generateEmailPage = _serviceFactory.GetService<IGenerateEmailPage>();
            _emailSenderOptions = _serviceFactory.GetService<EmailSenderOptions>();
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ecommerce", _emailSenderOptions.Username));
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


        public async Task<bool> VerifyEmailAddress(string emailAddress)
        {
            using (var httpClient = new HttpClient())
            {
                string parameters = $"api_key={_zeroBounce.ApiKey}&email={emailAddress}";
                HttpResponseMessage response = await httpClient.GetAsync($"{_zeroBounce.Url}?{parameters}");
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic getResponse = JsonConvert.DeserializeObject<dynamic>(responseContent).status;
                if (getResponse == "valid")
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> RegistrationMail(ApplicationUser user)
        {
            var page = _serviceFactory.GetService<IGenerateEmailPage>().EmailVerificationPage;
            string validToken = await _serviceFactory.GetService<IOtpService>().GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.EmailConfirmation);

            string appUrl = $"{_appConstants.AppUrl}/api/Auth/confirm-email?token={validToken}";
            await SendEmailAsync(user.Email, "Confirm your email", page(user.FirstName, appUrl));

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
