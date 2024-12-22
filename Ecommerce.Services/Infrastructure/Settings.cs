using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Configurations.Jwt;
using Ecommerce.Services.Extensions;

namespace Ecommerce.Services.Infrastructure
{
    public class Settings
    {
        public JwtConfig JwtConfig { get; set; }
        public RedisConfig redisConfig { get; set; }
        public CloudinarySettings CloudinarySettings { get; set; }
        public ZeroBounceConfig ZeroBounceConfig { get; set; }
        public EmailSenderOptions? EmailSenderOptions { get; set; }
        public FacebookConfig FacebookConfig { get; set; }
        public GoogleConfig GoogleConfig { get; set; }
    }

}
