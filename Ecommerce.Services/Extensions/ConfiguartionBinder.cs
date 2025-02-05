using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Email;
using Ecommerce.Services.Configurations.Jwt;
using Ecommerce.Services.Infrastructure;
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
            PaystackConfig paystackConfig = new();
            MicrosoftConfig microsoftConfig = new();
            ZeroBounceConfig zeroBounceConfig = new();
            FlutterwaveConfig flutterwaveConfig = new();
            EmailSenderOptions emailSenderOptions = new();
            CloudinarySettings cloudinarySettings = new();

            configuration.GetSection("JwtConfig").Bind(jwt);
            configuration.GetSection("RedisConfig").Bind(redisConfig);
            configuration.GetSection("AppConstants").Bind(appConstants);
            configuration.GetSection("GoogleConfig").Bind(googleConfig);
            configuration.GetSection("FacebookConfig").Bind(facebookConfig);
            configuration.GetSection("PaystackConfig").Bind(paystackConfig);
            configuration.GetSection("MicrosoftConfig").Bind(microsoftConfig);
            configuration.GetSection("ZeroBounceConfig").Bind(zeroBounceConfig);
            configuration.GetSection("FlutterwaveConfig").Bind(flutterwaveConfig);
            configuration.GetSection("EmailSenderOptions").Bind(emailSenderOptions);
            configuration.GetSection("CloudinarySettings").Bind(cloudinarySettings);

            services.AddSingleton(jwt);
            services.AddSingleton(redisConfig);
            services.AddSingleton(appConstants);
            services.AddSingleton(googleConfig);
            services.AddSingleton(facebookConfig);
            services.AddSingleton(paystackConfig);
            services.AddSingleton(microsoftConfig);
            services.AddSingleton(zeroBounceConfig);
            services.AddSingleton(flutterwaveConfig);
            services.AddSingleton(cloudinarySettings);
            services.AddSingleton(emailSenderOptions);

            return services;
        }
    }
}
