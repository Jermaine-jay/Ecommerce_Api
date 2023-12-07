using Ecommerce.Data.Context;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace Ecommerce.Data.Seeds
{
    public static class DatabaseSeedRoleClaims
    {
        public static async Task ClaimSeeder(this IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ApplicationDbContext>();


            using (var scope = app.ApplicationServices.CreateScope())
            {

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                context.Database.EnsureCreated();
                var claims = context.RoleClaims.Any();

                var user = await roleManager.FindByNameAsync(UserType.User.GetStringValue());
                var admin = await roleManager.FindByNameAsync(UserType.Admin.GetStringValue());
                var superadmin = await roleManager.FindByNameAsync(UserType.SuperAdmin.GetStringValue());

                if (!claims)
                {
                    await context.RoleClaims.AddRangeAsync(await UserClaim(user));
                    await context.RoleClaims.AddRangeAsync(await AdminClaim(admin));
                    await context.RoleClaims.AddRangeAsync(await AdminClaim(superadmin));
                    await context.SaveChangesAsync();
                }
            }
        }



        private static async Task<ICollection<ApplicationRoleClaim>> UserClaim(ApplicationRole role)
        {
            return new List<ApplicationRoleClaim>()
            {
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "change-password",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "profile",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-account",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-account",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-cart",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-to-cart",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-from-cart",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-cartitems",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "flutterwavepayment",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "verifyflutterwavepayment",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "clearcart",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "createorder",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "shippingaddress",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "available-system",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "paystackpayment",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "verifypaystackpayment",
                },
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "verifybankcharge",
                },
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-account",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "available-banks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "directcardpayment",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "directcardpayment",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "paystack-bank-charge",
                },

            };
        }


        private static async Task<ICollection<ApplicationRoleClaim>> AdminClaim(ApplicationRole role)
        {
            return new List<ApplicationRoleClaim>()
            {
                new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "all-users",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "remove-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "lock-user",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "create-role",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-user-role",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "remove-user-role",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "edit-role",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-role",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-roles",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-user-roles",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-claims",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-claim",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-claim",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "edit-claim",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-all-routes",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "create-category",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "available-system",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "available-banks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "get-all-routes",
                }
                ,new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "profile",
                }

            };
        }
    }
}
