using Ecommerce.Data.Context;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthServices, AuthServices>();
        }


        public static void ConfigureCors(this IServiceCollection services) =>
             services.AddCors(options =>
             {
                 options.AddPolicy("CorsPolicy", builder =>
                 builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
             });

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


        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
           
           
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
        }
}
