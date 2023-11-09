﻿using Ecommerce.Data.Context;
using Ecommerce.Data.Implementations;
using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Configurations.Jwt;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Authentication;
using System.Text;


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
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPaystackPaymentService, PaystackPaymentService>();
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


        public static void ConfigureJWT(this IServiceCollection services, JwtConfig jwtConfig)
        {
            var jwtSettings = jwtConfig;
            var secretKey = jwtSettings.Secret;

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                };
            });

            /* services.AddAuthorization(options =>
             {
                 options.AddPolicy("Authorization", policy =>
                 {
                     policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                     policy.RequireAuthenticatedUser();
                     //policy.Requirements.Add(new AuthRequirement());
                     policy.Build();
                 });
             });*/
        }

        public static void AddRedisCache(this IServiceCollection services, RedisConfig redisConfig)
        {

            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            configurationOptions.SslProtocols = SslProtocols.Tls12;
            configurationOptions.SyncTimeout = 30000;
            configurationOptions.Ssl = true;
            configurationOptions.Password = redisConfig.Password;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.EndPoints.Add(redisConfig.Host, redisConfig.Port);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configurationOptions.ToString();
                options.InstanceName = redisConfig.InstanceId;
            });

            services.AddSingleton<IConnectionMultiplexer>((x) =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    Password = configurationOptions.Password,
                    EndPoints = { configurationOptions.EndPoints[0] },
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    ClientName = redisConfig.InstanceId
                });
                return connectionMultiplexer;
            });
            services.AddTransient<ICacheService, CacheService>();
        }
    }
}
