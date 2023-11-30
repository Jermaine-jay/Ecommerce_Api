using Ecommerce.Data.Context;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace Ecommerce.Data.Seeds
{
    public static class SeedProducts
    {
        public static async Task ProductSeeder(this IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ApplicationDbContext>();


            using (var scope = app.ApplicationServices.CreateScope())
            {

                context.Database.EnsureCreated();
                var project = context.Products.Any();



                await context.Products.AddRangeAsync(GetProject());
                await context.SaveChangesAsync();

            }
        }


        private static ICollection<Product> GetProject()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    Description = "Test",
                    ProductVariation = null

                },

                new Product()
                {
                    Name = "Test2",
                    Description = "Test2",
                    ProductVariation = null

                },
                new Product()
                {
                    Name = "Test3",
                    Description = "Test3",
                    ProductVariation = null
                },

            };
        }
    }
}
