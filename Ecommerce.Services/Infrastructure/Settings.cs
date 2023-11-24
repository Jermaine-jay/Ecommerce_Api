using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Configurations.Jwt;

namespace Ecommerce.Services.Infrastructure
{
    public class Settings
    {
        public Authentication? Authentication { get; set; }
        public JwtConfig? JwtConfig { get; set; }
        public RedisConfig? redisConfig { get; set; }
        public ZeroBounceConfig? ZeroBounceConfig { get; set; }
        public EmailSenderOptions? EmailSenderOptions { get; set; }
        public CloudinarySettings? CloudinarySettings { get; set; }
    }


    public class Authentication
    {
        public Google Google { get; set; }
        public Facebook Facebook { get; set; }
    }

    public class Google
    {
        public string? ClientSecret { get; set; }
        public string? ClientId { get; set; }
    }

    public class Facebook
    {
        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
    }


    public class CloudinarySettings
    {
        public string? CloudName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    }

}
