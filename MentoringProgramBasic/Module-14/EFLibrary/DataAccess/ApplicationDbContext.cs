using InfrastructureLibraryAdoEfDapper.Models;
using Microsoft.EntityFrameworkCore;

namespace EFLibrary.DataAccess;

public class ApplicationDbContext : DbContext, IApplicationContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProductEntity(modelBuilder);
        ConfigureOrderEntity(modelBuilder);
    }

    private static void ConfigureProductEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.Weight)
                .HasPrecision(10, 2);

            entity.Property(e => e.Height)
                .HasPrecision(10, 2);

            entity.Property(e => e.Width)
                .HasPrecision(10, 2);

            entity.Property(e => e.Length)
                .HasPrecision(10, 2);
        });
    }

    private static void ConfigureOrderEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.CreateDate)
                .IsRequired();

            entity.Property(e => e.UpdateDate)
                .IsRequired();

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
