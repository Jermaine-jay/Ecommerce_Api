using TaskManager.Services.Configurations.Jwt;

namespace Ecommerce.Services.Infrastructure
{
    public class Settings
    {
        public Authentication? Authentication { get; set; } 
        public JwtConfig? JwtConfig { get; set; }
        public RedisConfig? redisConfig { get; set; }
        public ZeroBounceConfig? ZeroBounceConfig { get; set; }
        public EmailSenderOptions? EmailSenderOptions { get; set; }
        public Cloudinary? Cloudinary { get; set; }
    }

    public class EmailSenderOptions
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }

    public class ZeroBounceConfig
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
    }

    public class RedisConfig
    {
        public string InstanceId { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string IP { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Port { get; set; }
    }

    public class Authentication
    {
        public Google Google { get; set; }
        public Facebook Facebook { get; set; }
    }

    public class Google
    {
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
    }

    public class Facebook
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }


    public class Cloudinary
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

}
