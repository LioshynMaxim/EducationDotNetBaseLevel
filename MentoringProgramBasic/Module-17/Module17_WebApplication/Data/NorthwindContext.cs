using Microsoft.EntityFrameworkCore;
using Module17_WebApplication.Models;

namespace Module17_WebApplication.Data;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(15);
            entity.HasIndex(e => e.CategoryName);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(40);

            entity.HasIndex(e => e.ProductName);
            entity.HasIndex(e => e.CategoryID);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryID = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas, beers, and ales" },
            new Category { CategoryID = 2, CategoryName = "Condiments", Description = "Sweet and savory sauces, relishes, spreads, and seasonings" },
            new Category { CategoryID = 3, CategoryName = "Confections", Description = "Desserts, candies, and sweet breads" },
            new Category { CategoryID = 4, CategoryName = "Dairy Products", Description = "Cheeses" },
            new Category { CategoryID = 5, CategoryName = "Grains/Cereals", Description = "Breads, crackers, pasta, and cereal" },
            new Category { CategoryID = 6, CategoryName = "Meat/Poultry", Description = "Prepared meats" },
            new Category { CategoryID = 7, CategoryName = "Produce", Description = "Dried fruit and bean curd" },
            new Category { CategoryID = 8, CategoryName = "Seafood", Description = "Seaweed and fish" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductID = 1, ProductName = "Chai", CategoryID = 1, QuantityPerUnit = "10 boxes x 20 bags", UnitPrice = 18.00m, UnitsInStock = 39, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false },
            new Product { ProductID = 2, ProductName = "Chang", CategoryID = 1, QuantityPerUnit = "24 - 12 oz bottles", UnitPrice = 19.00m, UnitsInStock = 17, UnitsOnOrder = 40, ReorderLevel = 25, Discontinued = false },
            new Product { ProductID = 3, ProductName = "Aniseed Syrup", CategoryID = 2, QuantityPerUnit = "12 - 550 ml bottles", UnitPrice = 10.00m, UnitsInStock = 13, UnitsOnOrder = 70, ReorderLevel = 25, Discontinued = false },
            new Product { ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", CategoryID = 2, QuantityPerUnit = "48 - 6 oz jars", UnitPrice = 22.00m, UnitsInStock = 53, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false },
            new Product { ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", CategoryID = 2, QuantityPerUnit = "36 boxes", UnitPrice = 21.35m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true },
            new Product { ProductID = 6, ProductName = "Grandma's Boysenberry Spread", CategoryID = 2, QuantityPerUnit = "12 - 8 oz jars", UnitPrice = 25.00m, UnitsInStock = 120, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false },
            new Product { ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", CategoryID = 7, QuantityPerUnit = "12 - 1 lb pkgs.", UnitPrice = 30.00m, UnitsInStock = 15, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false },
            new Product { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", CategoryID = 2, QuantityPerUnit = "12 - 12 oz jars", UnitPrice = 40.00m, UnitsInStock = 6, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false },
            new Product { ProductID = 9, ProductName = "Mishi Kobe Niku", CategoryID = 6, QuantityPerUnit = "18 - 500 g pkgs.", UnitPrice = 97.00m, UnitsInStock = 29, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true },
            new Product { ProductID = 10, ProductName = "Ikura", CategoryID = 8, QuantityPerUnit = "12 - 200 ml jars", UnitPrice = 31.00m, UnitsInStock = 31, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false }
        );
    }
}
