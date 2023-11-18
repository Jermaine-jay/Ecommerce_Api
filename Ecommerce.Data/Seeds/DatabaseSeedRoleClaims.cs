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
                var role = await roleManager.FindByNameAsync(UserType.SuperAdmin.GetStringValue());

                if (!claims)
                {
                    await context.RoleClaims.AddRangeAsync(await UserClaim(user));
                    await context.RoleClaims.AddRangeAsync(await AdminClaim(admin));
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
                },/*new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "user-tasks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "user-projects",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "project-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-user-tasks",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "add-notifications",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "create-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "delete-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-task",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-priority",
                },new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "update-status",
                },*/

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
                    ClaimType = "get-a-user",
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
                }

            };
        }
    }
}
