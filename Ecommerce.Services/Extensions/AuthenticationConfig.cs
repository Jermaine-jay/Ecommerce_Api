namespace Ecommerce.Services.Extensions
{
    public class AuthenticationConfig
    {
        public FacebookConfig facebook;
        public GoogleConfig Google;
    }

    public class FacebookConfig
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }

    public class GoogleConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class MicrosoftConfig
    {
        public string Audience { get; set; }
        public string Tenanat { get; set; }
    }

    public class FlutterwaveConfig
    {
        public string ApiKey { get; set; }
    }

    public class PaystackConfig
    {
        public string ApiKey { get; set; }
    }
}
