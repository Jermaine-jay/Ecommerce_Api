namespace Ecommerce.Services.Interfaces
{
    public interface IGenerateEmailPage
    {
        string EmailVerificationPage(string name, string token);
        string PasswordResetPage(string callbackurl);
    }
}
