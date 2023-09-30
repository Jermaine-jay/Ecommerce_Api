using Ecommerce.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ProductImage>()
                .HasKey(p=>p.PublicId);

            modelBuilder.Entity<ProductVariation>()
                .Property(p => p.Price)
                .HasPrecision(16, 2);


            modelBuilder.Entity<ProductVariation>()
                .HasMany(ci => ci.ProductImages)
                .WithOne(p => p.ProductVariation)
                .HasForeignKey(ci => ci.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Order>()
                .HasOne(ci => ci.ShippingAddress)
                .WithOne(p => p.Order)
                .HasForeignKey<Order>(ci => ci.ShippingAddressId)
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Product>()
                .HasOne(ci => ci.ProductVariation)
                .WithOne(p => p.Product)
                .HasForeignKey<Product>(ci => ci.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Order>()
                .HasMany(ci => ci.OrderItems)
                .WithOne(p => p.Order)
                .HasForeignKey(ci => ci.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasOne(ci => ci.Cart)
                .WithOne(p => p.User)
                .HasForeignKey<Cart>(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /*https://localhost:44317/sigin-google*/

            modelBuilder.Entity<Cart>()
                .HasMany(ci => ci.CartItems)
                .WithOne(c => c.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Category>()
                .HasMany(oi => oi.Products)
                .WithOne(o => o.Category)
                .HasForeignKey(oi => oi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.HasMany<ApplicationUserRole>()
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            });


            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany<ApplicationUserRole>()
               .WithOne()
               .HasForeignKey(ur => ur.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
    }
}