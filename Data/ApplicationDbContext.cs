using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkaftoBageriA.Models;

namespace SkaftoBageriA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map "inventory" explicitly
            modelBuilder.Entity<Inventory>().ToTable("inventory");

            // Relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Supplier)
                .WithMany(s => s.Inventories)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
       .HasOne(p => p.Supplier)
       .WithMany(s => s.Products)
       .HasForeignKey(p => p.SupplierId)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Supplier)
                .WithMany(s => s.Inventories)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
            // Constraints
            modelBuilder.Entity<Product>().Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.ProductPrice).IsRequired().HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.ProductStock).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.ReorderPoint).IsRequired();

            modelBuilder.Entity<Product>()
       .HasOne(p => p.Supplier)
       .WithMany(s => s.Products)
       .HasForeignKey(p => p.SupplierId)
       .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Products

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Inventory

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Supplier)
                .WithMany(s => s.Inventories)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Supplier>().Property(s => s.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Inventory>().Property(i => i.BatchNumber).IsRequired().HasMaxLength(50);
        }
    }
}
