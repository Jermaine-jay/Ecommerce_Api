using Ecommerce.Data.Context;
using Ecommerce.Data.Implementations;
using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Cache.Security;
using Ecommerce.Services.Configurations.Jwt;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Attribute;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Authentication;
using System.Text;
using TaskManager.Services.Implementations;


namespace Ecommerce_Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IJwtAuthenticator, JwtAuthenticator>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRoleClaimService, RoleClaimService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthorizationHandler, AuthHandler>();
            services.AddScoped<IPaystackPaymentService, PaystackPaymentService>();
            services.AddScoped<ILoginAttempt, LoginAttempt>();
            services.AddScoped<IFlutterwavePaymentService, FlutterwavePaymentService>();
        }


        public static void RegisterDbContext(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseNpgsql(connectionString, s =>
                {
                    s.MigrationsAssembly("Ecommerce.Migrations");
                    s.EnableRetryOnFailure(3);
                });
            });
        }


        public static void ConfigureIdentity(this IServiceCollection services)
        {

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.SignIn.RequireConfirmedAccount = false;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
        }

        public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/LoginUser";
                })

                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                });
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration jwtConfig)
        {
            var secretKey = jwtConfig["JwtConfig:Secret"];

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["JwtConfig:Issuer"],
                    ValidAudience = jwtConfig["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authorization", policy =>
                {
                    policy.Requirements.Add(new AuthRequirement());
                });
            });

        }

        public static void AddRedisCache(this IServiceCollection services, IConfiguration redisConfig)
        {

            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = redisConfig["RedisConfig:Password"];
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(redisConfig["RedisConfig:Host"], int.Parse(redisConfig["RedisConfig:Port"]));
            configurationOptions.User = redisConfig["user"];

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = redisConfig["RedisConfig:InstanceId"];
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    User = configurationOptions.User,
                    Ssl = configurationOptions.Ssl,
                    SslProtocols = configurationOptions.SslProtocols
                });
                return connectionMultiplexer;
            });
            services.AddTransient<ICacheService, CacheService>();
        }
    }
}
