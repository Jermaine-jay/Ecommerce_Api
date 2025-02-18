using Ecommerce.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
     ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<ProductImage>(e =>
            {
                e.HasKey(p => p.PublicId);
                e.Property(e => e.PublicId)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ProductVariation>(e =>
            {
                e.Property(p => p.Price)
                .HasPrecision(16, 2);

                e.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ProductVariation>(e =>
            {
                e.HasMany(ci => ci.ProductImages)
                .WithOne(p => p.ProductVariation)
                .HasForeignKey(ci => ci.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade);

                e.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });


            modelBuilder.Entity<Order>()
                .HasOne(ci => ci.ShippingAddress)
                .WithOne(p => p.Order)
                .HasForeignKey<Order>(ci => ci.ShippingAddressId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasMany(ci => ci.ProductVariation)
                .WithOne(p => p.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });


            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasMany(ci => ci.OrderItems)
                    .WithOne(p => p.Order)
                    .HasForeignKey(ci => ci.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(ci => ci.Orders)
                .WithOne(p => p.User)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Category>(e =>
            {
                e.HasMany(oi => oi.Products)
                .WithOne(o => o.Category)
                .HasForeignKey(oi => oi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                e.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
            });

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
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
    }
}