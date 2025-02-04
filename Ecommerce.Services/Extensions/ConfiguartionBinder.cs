using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Configurations.Jwt;
using Ecommerce.Services.Infrastructure;
using Flutterwave.Net.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppConstants = Ecommerce.Services.Infrastructure.AppConstants;

namespace Ecommerce.Services.Extensions
{
    public static class ConfiguartionBinder
    {
        public static IServiceCollection BindConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            JwtConfig jwt = new();
            RedisConfig redisConfig = new();
            AppConstants appConstants = new();
            GoogleConfig googleConfig = new();
            FacebookConfig facebookConfig = new();
            ZeroBounceConfig zeroBounceConfig = new();
            CloudinarySettings cloudinarySettings = new();
            EmailSenderOptions emailSenderOptions = new();

            configuration.GetSection("JwtConfig").Bind(jwt);
            configuration.GetSection("RedisConfig").Bind(redisConfig);
            configuration.GetSection("AppConstants").Bind(appConstants);
            configuration.GetSection("EmailSenderOptions").Bind(emailSenderOptions);
            configuration.GetSection("ZeroBounceConfig").Bind(zeroBounceConfig);
            configuration.GetSection("GoogleConfig").Bind(googleConfig);
            configuration.GetSection("FacebookConfig").Bind(facebookConfig);
            configuration.GetSection("CloudinarySettings").Bind(cloudinarySettings);

            services.AddSingleton(jwt);
            services.AddSingleton(redisConfig);
            services.AddSingleton(appConstants);
            services.AddSingleton(emailSenderOptions);
            services.AddSingleton(googleConfig);
            services.AddSingleton(facebookConfig);
            services.AddSingleton(zeroBounceConfig);
            services.AddSingleton(cloudinarySettings);

            return services;
        }
    }
}
