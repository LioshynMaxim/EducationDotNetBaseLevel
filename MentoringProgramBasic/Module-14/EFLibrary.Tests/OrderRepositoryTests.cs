using EFLibrary.DataAccess;
using EFLibrary.Repositories;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFLibrary.Tests;

public class OrderRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly OrderRepository _repository;
    private readonly ProductRepository _productRepository;
    private readonly Product _testProduct;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _productRepository = new ProductRepository(_context);
        _repository = new OrderRepository(_context, _productRepository);

        _testProduct = new Product
        {
            Name = "Test Product",
            Description = "For testing orders",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        _context.Products.Add(_testProduct);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderRepository(null!, _productRepository));
    }

    [Fact]
    public void Constructor_WithNullProductRepository_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderRepository(_context, null!));
    }

    [Fact]
    public void Create_WithValidOrder_ShouldAddToDatabase()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };

        _repository.Create(order);

        var savedOrder = _context.Orders.FirstOrDefault(o => o.ProductId == _testProduct.Id);
        Assert.NotNull(savedOrder);
        Assert.Equal(OrderStatus.NotStarted, savedOrder.Status);
        Assert.True(savedOrder.Id > 0);
    }

    [Fact]
    public void Create_WithInvalidProductId_ShouldThrowArgumentException()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 999
        };

        Assert.Throws<ArgumentException>(() => _repository.Create(order));
    }

    [Fact]
    public void Create_WithMultipleOrders_ShouldAddAll()
    {
        var orders = new[]
        {
            new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Done, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id }
        };

        foreach (var order in orders)
        {
            _repository.Create(order);
        }

        Assert.Equal(3, _context.Orders.Count());
    }

    [Fact]
    public void Create_ShouldPreserveDates()
    {
        var createDate = new DateTime(2024, 1, 1, 10, 0, 0);
        var updateDate = new DateTime(2024, 1, 2, 11, 0, 0);
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = createDate,
            UpdateDate = updateDate,
            ProductId = _testProduct.Id
        };

        _repository.Create(order);

        var saved = _context.Orders.Find(order.Id);
        Assert.Equal(createDate, saved!.CreateDate);
        Assert.Equal(updateDate, saved.UpdateDate);
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnOrderWithProduct()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        var result = _repository.Read(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.NotNull(result.Product);
        Assert.Equal(_testProduct.Id, result.Product.Id);
        Assert.Equal("Test Product", result.Product.Name);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        var result = _repository.Read(999);

        Assert.Null(result);
    }

    [Fact]
    public void Read_ShouldLoadAllOrderProperties()
    {
        var order = new Order
        {
            Status = OrderStatus.InProgress,
            CreateDate = new DateTime(2024, 3, 15),
            UpdateDate = new DateTime(2024, 3, 16),
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        var result = _repository.Read(order.Id);

        Assert.Equal(OrderStatus.InProgress, result!.Status);
        Assert.Equal(new DateTime(2024, 3, 15), result.CreateDate);
        Assert.Equal(new DateTime(2024, 3, 16), result.UpdateDate);
    }

    [Fact]
    public void Update_WithValidOrder_ShouldUpdateDatabase()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        order.Status = OrderStatus.InProgress;
        order.UpdateDate = DateTime.Now.AddHours(1);
        _repository.Update(order);

        var updated = _context.Orders.Find(order.Id);
        Assert.NotNull(updated);
        Assert.Equal(OrderStatus.InProgress, updated.Status);
    }

    [Fact]
    public void Update_WithInvalidProductId_ShouldThrowArgumentException()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        order.ProductId = 999;
        
        Assert.Throws<ArgumentException>(() => _repository.Update(order));
    }

    [Fact]
    public void Update_ShouldPreserveCreateDate()
    {
        var createDate = new DateTime(2024, 1, 1);
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = createDate,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        order.Status = OrderStatus.Done;
        order.UpdateDate = DateTime.Now.AddDays(10);
        _repository.Update(order);

        var updated = _context.Orders.Find(order.Id);
        Assert.Equal(createDate, updated!.CreateDate);
        Assert.Equal(OrderStatus.Done, updated.Status);
    }

    [Fact]
    public void Delete_WithValidId_ShouldRemoveFromDatabase()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();
        var orderId = order.Id;

        _repository.Delete(orderId);

        var deleted = _context.Orders.Find(orderId);
        Assert.Null(deleted);
    }

    [Fact]
    public void Delete_WithInvalidId_ShouldNotThrow()
    {
        var exception = Record.Exception(() => _repository.Delete(999));
        
        Assert.Null(exception);
    }

    [Fact]
    public void Delete_ShouldNotDeleteProduct()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = _testProduct.Id
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        _repository.Delete(order.Id);

        var product = _context.Products.Find(_testProduct.Id);
        Assert.NotNull(product);
    }

    [Fact]
    public void GetAll_ShouldReturnAllOrdersWithProducts()
    {
        var orders = new[]
        {
            new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Done, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id }
        };
        _context.Orders.AddRange(orders);
        _context.SaveChanges();

        var result = _repository.GetAll().ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, order => Assert.NotNull(order.Product));
        Assert.All(result, order => Assert.Equal("Test Product", order.Product!.Name));
    }

    [Fact]
    public void GetAll_WhenEmpty_ShouldReturnEmptyCollection()
    {
        var result = _repository.GetAll().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_WithMultipleProducts_ShouldLoadCorrectProducts()
    {
        var product2 = new Product { Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m };
        _context.Products.Add(product2);
        _context.SaveChanges();

        var order1 = new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id };
        var order2 = new Order { Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = product2.Id };
        _context.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var result = _repository.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, o => o.Product!.Name == "Test Product");
        Assert.Contains(result, o => o.Product!.Name == "Product 2");
    }

    [Fact]
    public void GetOrders_WithNoFilters_ShouldReturnAllOrders()
    {
        var orders = CreateTestOrders();

        var result = _repository.GetOrders(o => true).ToList();

        Assert.Equal(orders.Length, result.Count);
    }

    [Fact]
    public void GetOrders_WithProductIdFilter_ShouldReturnMatchingOrders()
    {
        var product2 = new Product { Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m };
        _context.Products.Add(product2);
        _context.SaveChanges();

        CreateTestOrders();
        _context.Orders.Add(new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = product2.Id });
        _context.SaveChanges();

        var result = _repository.GetOrders(o => o.ProductId == _testProduct.Id).ToList();

        Assert.All(result, o => Assert.Equal(_testProduct.Id, o.ProductId));
        Assert.DoesNotContain(result, o => o.ProductId == product2.Id);
    }

    [Fact]
    public void GetOrders_WithStatusFilter_ShouldReturnMatchingOrders()
    {
        CreateTestOrders();

        var result = _repository.GetOrders(o => o.Status == OrderStatus.InProgress).ToList();

        Assert.All(result, o => Assert.Equal(OrderStatus.InProgress, o.Status));
    }

    [Fact]
    public void GetOrders_WithYearFilter_ShouldReturnMatchingOrders()
    {
        _context.Orders.Add(new Order { Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 1, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id });
        _context.Orders.Add(new Order { Status = OrderStatus.NotStarted, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id });
        _context.SaveChanges();

        var result = _repository.GetOrders(o => o.CreateDate.Year == 2024).ToList();

        Assert.All(result, o => Assert.Equal(2024, o.CreateDate.Year));
        Assert.Single(result);
    }

    [Fact]
    public void GetOrders_WithMonthFilter_ShouldReturnMatchingOrders()
    {
        _context.Orders.Add(new Order { Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id });
        _context.Orders.Add(new Order { Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 4, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id });
        _context.SaveChanges();

        var result = _repository.GetOrders(o => o.CreateDate.Month == 3).ToList();

        Assert.All(result, o => Assert.Equal(3, o.CreateDate.Month));
        Assert.Single(result);
    }

    [Fact]
    public void GetOrders_WithMultipleFilters_ShouldReturnMatchingOrders()
    {
        _context.Orders.AddRange(
            new Order { Status = OrderStatus.InProgress, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.InProgress, CreateDate = new DateTime(2024, 4, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Done, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id }
        );
        _context.SaveChanges();

        var result = _repository.GetOrders(o => 
            o.ProductId == _testProduct.Id &&
            o.Status == OrderStatus.InProgress &&
            o.CreateDate.Year == 2024 &&
            o.CreateDate.Month == 3).ToList();

        Assert.Single(result);
        Assert.Equal(OrderStatus.InProgress, result[0].Status);
        Assert.Equal(3, result[0].CreateDate.Month);
    }

    [Fact]
    public void GetOrders_WithNonMatchingFilters_ShouldReturnEmpty()
    {
        CreateTestOrders();

        var result = _repository.GetOrders(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 1999).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void DeleteOrdersBulk_WithNoFilters_ShouldDeleteAllOrders()
    {
        CreateTestOrders();
        var initialCount = _context.Orders.Count();

        var deleted = _repository.DeleteOrdersBulk(o => true);

        Assert.Equal(initialCount, deleted);
        Assert.Empty(_context.Orders);
    }

    [Fact]
    public void DeleteOrdersBulk_WithStatusFilter_ShouldDeleteMatchingOrders()
    {
        CreateTestOrders();

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.InProgress);

        var remaining = _context.Orders.ToList();
        Assert.True(deleted > 0);
        Assert.DoesNotContain(remaining, o => o.Status == OrderStatus.InProgress);
    }

    [Fact]
    public void DeleteOrdersBulk_WithMultipleFilters_ShouldDeleteMatchingOrders()
    {
        _context.Orders.AddRange(
            new Order { Status = OrderStatus.Cancelled, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Cancelled, CreateDate = new DateTime(2024, 1, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Done, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id }
        );
        _context.SaveChanges();

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 2023);

        Assert.Equal(1, deleted);
        Assert.Equal(2, _context.Orders.Count());
    }

    [Fact]
    public void DeleteOrdersBulk_WhenNoOrdersMatch_ShouldReturnZero()
    {
        CreateTestOrders();

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 1999);

        Assert.Equal(0, deleted);
    }

    [Fact]
    public void DeleteOrdersBulk_ShouldNotDeleteProducts()
    {
        CreateTestOrders();
        var productCount = _context.Products.Count();

        _repository.DeleteOrdersBulk(o => true);

        Assert.Equal(productCount, _context.Products.Count());
    }

    [Fact]
    public void GetOrders_ShouldIncludeProductNavigation()
    {
        CreateTestOrders();

        var result = _repository.GetOrders(o => true).ToList();

        Assert.All(result, order =>
        {
            Assert.NotNull(order.Product);
            Assert.NotEmpty(order.Product.Name);
        });
    }

    [Fact]
    public void CRUD_Integration_ShouldWorkTogether()
    {
        var order = new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = _testProduct.Id };
        
        _repository.Create(order);
        var created = _repository.Read(order.Id);
        Assert.NotNull(created);
        Assert.Equal(OrderStatus.NotStarted, created.Status);

        created.Status = OrderStatus.Done;
        _repository.Update(created);
        var updated = _repository.Read(order.Id);
        Assert.Equal(OrderStatus.Done, updated!.Status);

        _repository.Delete(order.Id);
        var deleted = _repository.Read(order.Id);
        Assert.Null(deleted);
    }

    private Order[] CreateTestOrders()
    {
        var orders = new[]
        {
            new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = _testProduct.Id },
            new Order { Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = _testProduct.Id }
        };
        _context.Orders.AddRange(orders);
        _context.SaveChanges();
        return orders;
    }
}
