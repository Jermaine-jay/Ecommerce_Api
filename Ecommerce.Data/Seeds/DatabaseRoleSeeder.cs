using Ecommerce.Data.Context;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace TaskManager.Data.Seeds
{
    public static class DatabaseRoleSeeder
    {
        public static async Task SeedRole(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                ApplicationDbContext context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                context.Database.EnsureCreated();
                var roleExist = context.Roles.Any();

                if (!roleExist)
                {
                    context.Roles.AddRange(await SeededRoles());
                    await context.SaveChangesAsync();
                }
            }
        }


        private static async Task<IEnumerable<ApplicationRole>> SeededRoles()
        {
            return new List<ApplicationRole>()
            {
                new ApplicationRole()
                {
                    Name = UserTypeExtension.GetStringValue(UserType.User),
                    Type = UserType.User,
                    NormalizedName = UserTypeExtension.GetStringValue(UserType.User)?.ToUpper()
                                                      .Normalize()
                },
                new ApplicationRole()
                {
                    Name = UserTypeExtension.GetStringValue(UserType.Admin),
                    Type = UserType.Admin,
                    NormalizedName = UserTypeExtension.GetStringValue(UserType.Admin)?.ToUpper()
                                                      .Normalize()
                },
                new ApplicationRole
                {
                    Name = UserTypeExtension.GetStringValue(UserType.SuperAdmin),
                    Type = UserType.SuperAdmin,
                    NormalizedName = UserTypeExtension.GetStringValue(UserType.SuperAdmin)?.ToUpper()
                                                      .Normalize()
                }
            };
        }
    }
}
