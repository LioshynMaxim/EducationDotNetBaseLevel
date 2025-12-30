using DapperLibrary.DataAccess;
using DapperLibrary.Repositories;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;
using Moq;
using Xunit;

namespace DapperLibrary.Tests;

public class OrderRepositoryTests
{
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        _mockDataAccess = new Mock<IDataAccess>();
        _mockProductRepository = new Mock<IProductRepository>();
        
        // Default setup - Product exists
        _mockProductRepository
            .Setup(p => p.Read(It.IsAny<int>()))
            .Returns(new Product { Id = 1, Name = "Test Product", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m });
        
        _repository = new OrderRepository(_mockDataAccess.Object, _mockProductRepository.Object);
    }

    [Fact]
    public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderRepository(null!, _mockProductRepository.Object));
    }

    [Fact]
    public void Constructor_WithNullProductRepository_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderRepository(_mockDataAccess.Object, null!));
    }

    [Fact]
    public void Create_WithValidOrder_ShouldCallExecute()
    {
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        _repository.Create(order);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _mockProductRepository.Verify(p => p.Read(1), Times.Once);
    }

    [Fact]
    public void Create_WithInvalidProductId_ShouldThrowArgumentException()
    {
        _mockProductRepository
            .Setup(p => p.Read(999))
            .Returns((Product?)null);

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
    public void Create_WithMultipleOrders_ShouldCallExecuteMultipleTimes()
    {
        var orders = new[]
        {
            new Order { Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Status = OrderStatus.Done, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1 }
        };

        foreach (var order in orders)
        {
            _repository.Create(order);
        }

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
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
            ProductId = 1
        };

        _repository.Create(order);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnOrderWithProduct()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1,
            Product = new Product { Id = 1, Name = "Test Product", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m }
        };

        _mockDataAccess
            .Setup(d => d.Execute<Order>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(order);

        var result = _repository.Read(1);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        _mockDataAccess
            .Setup(d => d.Execute<Order>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns((Order?)null);

        var result = _repository.Read(999);

        Assert.Null(result);
    }

    [Fact]
    public void Read_ShouldLoadAllOrderProperties()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = new DateTime(2024, 3, 15),
            UpdateDate = new DateTime(2024, 3, 16),
            ProductId = 1
        };

        _mockDataAccess
            .Setup(d => d.Execute<Order>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(order);

        var result = _repository.Read(1);

        Assert.Equal(OrderStatus.InProgress, result!.Status);
        Assert.Equal(new DateTime(2024, 3, 15), result.CreateDate);
        Assert.Equal(new DateTime(2024, 3, 16), result.UpdateDate);
    }

    [Fact]
    public void Update_WithValidOrder_ShouldCallExecute()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now.AddHours(1),
            ProductId = 1
        };

        _repository.Update(order);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        _mockProductRepository.Verify(p => p.Read(1), Times.Once);
    }

    [Fact]
    public void Update_WithInvalidProductId_ShouldThrowArgumentException()
    {
        _mockProductRepository
            .Setup(p => p.Read(999))
            .Returns((Product?)null);

        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 999
        };

        Assert.Throws<ArgumentException>(() => _repository.Update(order));
    }

    [Fact]
    public void Update_ShouldPreserveCreateDate()
    {
        var createDate = new DateTime(2024, 1, 1);
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Done,
            CreateDate = createDate,
            UpdateDate = DateTime.Now.AddDays(10),
            ProductId = 1
        };

        _repository.Update(order);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Delete_WithValidId_ShouldCallExecute()
    {
        const int orderId = 1;

        _repository.Delete(orderId);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Delete_WithInvalidId_ShouldNotThrow()
    {
        var exception = Record.Exception(() => _repository.Delete(999));
        
        Assert.Null(exception);
    }

    [Fact]
    public void Delete_MultipleTimes_ShouldCallExecuteMultipleTimes()
    {
        _repository.Delete(1);
        _repository.Delete(2);
        _repository.Delete(3);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
    }

    [Fact]
    public void GetAll_ShouldReturnAllOrdersWithProducts()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetAll().ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, order => Assert.NotNull(order.Product));
    }

    [Fact]
    public void GetAll_WhenEmpty_ShouldReturnEmptyCollection()
    {
        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(Array.Empty<Order>());

        var result = _repository.GetAll().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_WithMultipleProducts_ShouldLoadCorrectProducts()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 2, Product = new Product { Id = 2, Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m } }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, o => o.Product!.Name == "Product 1");
        Assert.Contains(result, o => o.Product!.Name == "Product 2");
    }

    [Fact]
    public void GetOrders_WithNoPredicate_ShouldReturnAllOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => true).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetOrders_WithProductIdFilter_ShouldReturnMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 2, Product = new Product { Id = 2, Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m } }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => o.ProductId == 1).ToList();

        Assert.All(result, o => Assert.Equal(1, o.ProductId));
        Assert.DoesNotContain(result, o => o.ProductId == 2);
    }

    [Fact]
    public void GetOrders_WithStatusFilter_ShouldReturnMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => o.Status == OrderStatus.InProgress).ToList();

        Assert.All(result, o => Assert.Equal(OrderStatus.InProgress, o.Status));
    }

    [Fact]
    public void GetOrders_WithYearFilter_ShouldReturnMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 1, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.NotStarted, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => o.CreateDate.Year == 2024).ToList();

        Assert.All(result, o => Assert.Equal(2024, o.CreateDate.Year));
        Assert.Single(result);
    }

    [Fact]
    public void GetOrders_WithMonthFilter_ShouldReturnMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.NotStarted, CreateDate = new DateTime(2024, 4, 1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => o.CreateDate.Month == 3).ToList();

        Assert.All(result, o => Assert.Equal(3, o.CreateDate.Month));
        Assert.Single(result);
    }

    [Fact]
    public void GetOrders_WithMultipleFilters_ShouldReturnMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.InProgress, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = new DateTime(2024, 4, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = new DateTime(2024, 3, 1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => 
            o.ProductId == 1 &&
            o.Status == OrderStatus.InProgress &&
            o.CreateDate.Year == 2024 &&
            o.CreateDate.Month == 3).ToList();

        Assert.Single(result);
        Assert.Equal(OrderStatus.InProgress, result[0].Status);
        Assert.Equal(3, result[0].CreateDate.Month);
    }

    [Fact]
    public void GetOrders_WithNonMatchingFilter_ShouldReturnEmpty()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var result = _repository.GetOrders(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 1999).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void DeleteOrdersBulk_WithTruePredicate_ShouldDeleteAllOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var deleted = _repository.DeleteOrdersBulk(o => true);

        Assert.Equal(3, deleted);
    }

    [Fact]
    public void DeleteOrdersBulk_WithStatusFilter_ShouldDeleteMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.InProgress);

        Assert.Equal(1, deleted);
    }

    [Fact]
    public void DeleteOrdersBulk_WithMultipleFilters_ShouldDeleteMatchingOrders()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.Cancelled, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.Cancelled, CreateDate = new DateTime(2024, 1, 1), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = new DateTime(2023, 1, 1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 2023);

        Assert.Equal(1, deleted);
    }

    [Fact]
    public void DeleteOrdersBulk_WhenNoOrdersMatch_ShouldReturnZero()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1 },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1 }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

        var deleted = _repository.DeleteOrdersBulk(o => o.Status == OrderStatus.Cancelled && o.CreateDate.Year == 1999);

        Assert.Equal(0, deleted);
    }

    [Fact]
    public void GetOrders_ShouldIncludeProductNavigation()
    {
        var orders = new[]
        {
            new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now.AddDays(-5), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 2, Status = OrderStatus.InProgress, CreateDate = DateTime.Now.AddDays(-3), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } },
            new Order { Id = 3, Status = OrderStatus.Done, CreateDate = DateTime.Now.AddDays(-1), UpdateDate = DateTime.Now, ProductId = 1, Product = new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m } }
        };

        _mockDataAccess
            .Setup(d => d.Query<Order>(It.IsAny<string>(), null))
            .Returns(orders);

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
        var product = new Product { Id = 1, Name = "Test Product", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        var order = new Order { Id = 1, Status = OrderStatus.NotStarted, CreateDate = DateTime.Now, UpdateDate = DateTime.Now, ProductId = 1 };
        
        // Create
        _repository.Create(order);
        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);

        // Read
        _mockDataAccess
            .Setup(d => d.Execute<Order>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(order);
        
        var created = _repository.Read(order.Id);
        Assert.NotNull(created);
        Assert.Equal(OrderStatus.NotStarted, created.Status);

        // Update
        order.Status = OrderStatus.Done;
        _repository.Update(order);
        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));

        // Delete
        _repository.Delete(order.Id);
        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
    }
}
