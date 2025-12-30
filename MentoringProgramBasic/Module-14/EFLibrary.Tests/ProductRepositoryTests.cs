using EFLibrary.DataAccess;
using EFLibrary.Repositories;
using InfrastructureLibraryAdoEfDapper.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFLibrary.Tests;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ProductRepository(null!));
    }

    [Fact]
    public void Create_WithValidProduct_ShouldAddToDatabase()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };

        _repository.Create(product);

        var savedProduct = _context.Products.FirstOrDefault(p => p.Name == "Test Product");
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Product", savedProduct.Name);
        Assert.Equal("Test Description", savedProduct.Description);
        Assert.Equal(1.5m, savedProduct.Weight);
        Assert.True(savedProduct.Id > 0);
    }

    [Fact]
    public void Create_WithMultipleProducts_ShouldAddAll()
    {
        var products = new[]
        {
            new Product { Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m },
            new Product { Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m },
            new Product { Name = "Product 3", Weight = 3.0m, Height = 3.0m, Width = 3.0m, Length = 3.0m }
        };

        foreach (var product in products)
        {
            _repository.Create(product);
        }

        Assert.Equal(3, _context.Products.Count());
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnProduct()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };
        _context.Products.Add(product);
        _context.SaveChanges();

        var result = _repository.Read(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(1.5m, result.Weight);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        var result = _repository.Read(999);

        Assert.Null(result);
    }

    [Fact]
    public void Read_AfterCreate_ShouldReturnCreatedProduct()
    {
        var product = new Product { Name = "New Product", Weight = 5.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        _repository.Create(product);

        var result = _repository.Read(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("New Product", result.Name);
    }

    [Fact]
    public void Update_WithValidProduct_ShouldUpdateDatabase()
    {
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            Weight = 1.0m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };
        _context.Products.Add(product);
        _context.SaveChanges();

        product.Name = "Updated Name";
        product.Weight = 2.5m;
        _repository.Update(product);

        var updated = _context.Products.Find(product.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal(2.5m, updated.Weight);
    }

    [Fact]
    public void Update_MultipleFields_ShouldUpdateAll()
    {
        var product = new Product { Name = "Old", Description = "Old Desc", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        _context.Products.Add(product);
        _context.SaveChanges();

        product.Name = "New";
        product.Description = "New Desc";
        product.Weight = 5.5m;
        product.Height = 6.6m;
        _repository.Update(product);

        var updated = _context.Products.Find(product.Id);
        Assert.Equal("New", updated!.Name);
        Assert.Equal("New Desc", updated.Description);
        Assert.Equal(5.5m, updated.Weight);
        Assert.Equal(6.6m, updated.Height);
    }

    [Fact]
    public void Delete_WithValidId_ShouldRemoveFromDatabase()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };
        _context.Products.Add(product);
        _context.SaveChanges();
        var productId = product.Id;

        _repository.Delete(productId);

        var deleted = _context.Products.Find(productId);
        Assert.Null(deleted);
    }

    [Fact]
    public void Delete_WithInvalidId_ShouldNotThrow()
    {
        var exception = Record.Exception(() => _repository.Delete(999));
        
        Assert.Null(exception);
    }

    [Fact]
    public void Delete_ShouldDecrementCount()
    {
        var product1 = new Product { Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        var product2 = new Product { Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m };
        _context.Products.AddRange(product1, product2);
        _context.SaveChanges();

        _repository.Delete(product1.Id);

        Assert.Equal(1, _context.Products.Count());
        Assert.NotNull(_context.Products.Find(product2.Id));
    }

    [Fact]
    public void GetAll_WithMultipleProducts_ShouldReturnAll()
    {
        var products = new[]
        {
            new Product { Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m },
            new Product { Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m },
            new Product { Name = "Product 3", Weight = 3.0m, Height = 3.0m, Width = 3.0m, Length = 3.0m }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();

        var result = _repository.GetAll().ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name == "Product 1");
        Assert.Contains(result, p => p.Name == "Product 2");
        Assert.Contains(result, p => p.Name == "Product 3");
    }

    [Fact]
    public void GetAll_WhenEmpty_ShouldReturnEmptyCollection()
    {
        var result = _repository.GetAll().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllProperties()
    {
        var product = new Product
        {
            Name = "Complete Product",
            Description = "Full Description",
            Weight = 1.5m,
            Height = 2.5m,
            Width = 3.5m,
            Length = 4.5m
        };
        _context.Products.Add(product);
        _context.SaveChanges();

        var result = _repository.GetAll().First();

        Assert.Equal("Complete Product", result.Name);
        Assert.Equal("Full Description", result.Description);
        Assert.Equal(1.5m, result.Weight);
        Assert.Equal(2.5m, result.Height);
        Assert.Equal(3.5m, result.Width);
        Assert.Equal(4.5m, result.Length);
    }

    [Fact]
    public void Create_Update_Read_Integration_ShouldWork()
    {
        var product = new Product { Name = "Initial", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        
        _repository.Create(product);
        var created = _repository.Read(product.Id);
        Assert.Equal("Initial", created!.Name);

        created.Name = "Modified";
        _repository.Update(created);
        var updated = _repository.Read(product.Id);
        Assert.Equal("Modified", updated!.Name);
    }

    [Fact]
    public void GetAll_AfterDeleteOne_ShouldReturnRemaining()
    {
        var products = new[]
        {
            new Product { Name = "Keep", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m },
            new Product { Name = "Delete", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();

        _repository.Delete(products[1].Id);
        var result = _repository.GetAll().ToList();

        Assert.Single(result);
        Assert.Equal("Keep", result[0].Name);
    }
}
