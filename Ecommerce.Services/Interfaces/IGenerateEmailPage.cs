namespace Ecommerce.Services.Interfaces
{
    public interface IGenerateEmailPage
    {
        string PasswordResetPage(string callbackurl);
    }
}
