using EFLibrary.DataAccess;
using InfrastructureLibraryAdoEfDapper.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFLibrary.Tests;

public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void DbSets_ShouldBeInitialized()
    {
        Assert.NotNull(_context.Products);
        Assert.NotNull(_context.Orders);
    }

    [Fact]
    public void Product_CanBeAdded()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        Assert.NotEqual(0, product.Id);
        Assert.Equal(1, _context.Products.Count());
        
        var savedProduct = _context.Products.FirstOrDefault(p => p.Name == "Test Product");
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Description", savedProduct.Description);
    }

    [Fact]
    public void Order_WithProduct_CanBeAdded()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = product.Id
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        Assert.NotEqual(0, order.Id);
        Assert.Equal(1, _context.Orders.Count());
        
        var savedOrder = _context.Orders.FirstOrDefault(o => o.ProductId == product.Id);
        Assert.NotNull(savedOrder);
        Assert.Equal(OrderStatus.NotStarted, savedOrder.Status);
    }

    [Fact]
    public void Order_ProductNavigation_CanBeLoaded()
    {
        var product = new Product 
        { 
            Name = "Test Product",
            Weight = 1.0m, 
            Height = 1.0m, 
            Width = 1.0m, 
            Length = 1.0m 
        };
        
        _context.Products.Add(product);
        _context.SaveChanges();

        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = product.Id
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        var loadedOrder = _context.Orders
            .Include(o => o.Product)
            .FirstOrDefault(o => o.Id == order.Id);

        Assert.NotNull(loadedOrder);
        Assert.NotNull(loadedOrder.Product);
        Assert.Equal("Test Product", loadedOrder.Product.Name);
    }
}
