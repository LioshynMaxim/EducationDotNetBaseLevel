using Microsoft.EntityFrameworkCore;
using Module16_WebApplication.Models;

namespace Module16_WebApplication.Data;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(15);
            entity.HasIndex(e => e.CategoryName);
        });
        
        // Configure Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierID);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(40);
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.PostalCode);
        });
        
        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(40);
            
            entity.HasIndex(e => e.ProductName);
            entity.HasIndex(e => e.CategoryID);
            entity.HasIndex(e => e.SupplierID);
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.SupplierID)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
