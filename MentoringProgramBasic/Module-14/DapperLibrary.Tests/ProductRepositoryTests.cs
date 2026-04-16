using DapperLibrary.DataAccess;
using DapperLibrary.Repositories;
using InfrastructureLibraryAdoEfDapper.Models;
using Moq;
using Xunit;

namespace DapperLibrary.Tests;

public class ProductRepositoryTests
{
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _mockDataAccess = new Mock<IDataAccess>();
        _repository = new ProductRepository(_mockDataAccess.Object);
    }

    [Fact]
    public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ProductRepository(null!));
    }

    [Fact]
    public void Create_WithValidProduct_ShouldCallExecute()
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

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Create_WithMultipleProducts_ShouldCallExecuteMultipleTimes()
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

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnProduct()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };

        _mockDataAccess
            .Setup(d => d.Execute<Product>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(product);

        var result = _repository.Read(1);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(1.5m, result.Weight);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        _mockDataAccess
            .Setup(d => d.Execute<Product>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns((Product?)null);

        var result = _repository.Read(999);

        Assert.Null(result);
    }

    [Fact]
    public void Read_WithValidId_ShouldCallExecute()
    {
        var product = new Product { Id = 1, Name = "Test", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        
        _mockDataAccess
            .Setup(d => d.Execute<Product>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(product);

        _repository.Read(1);

        _mockDataAccess.Verify(d => d.Execute<Product>(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Update_WithValidProduct_ShouldCallExecute()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Updated Name",
            Description = "Updated Description",
            Weight = 2.5m,
            Height = 3.0m,
            Width = 4.0m,
            Length = 5.0m
        };

        _repository.Update(product);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Update_MultipleFields_ShouldCallExecute()
    {
        var product = new Product 
        { 
            Id = 1,
            Name = "New", 
            Description = "New Desc", 
            Weight = 5.5m, 
            Height = 6.6m, 
            Width = 7.7m, 
            Length = 8.8m 
        };

        _repository.Update(product);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Delete_WithValidId_ShouldCallExecute()
    {
        const int productId = 1;

        _repository.Delete(productId);

        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
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
    public void GetAll_ShouldReturnAllProducts()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m },
            new Product { Id = 2, Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m },
            new Product { Id = 3, Name = "Product 3", Weight = 3.0m, Height = 3.0m, Width = 3.0m, Length = 3.0m }
        };

        _mockDataAccess
            .Setup(d => d.Query<Product>(It.IsAny<string>(), null))
            .Returns(products);

        var result = _repository.GetAll().ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name == "Product 1");
        Assert.Contains(result, p => p.Name == "Product 2");
        Assert.Contains(result, p => p.Name == "Product 3");
    }

    [Fact]
    public void GetAll_WhenEmpty_ShouldReturnEmptyCollection()
    {
        _mockDataAccess
            .Setup(d => d.Query<Product>(It.IsAny<string>(), null))
            .Returns(Array.Empty<Product>());

        var result = _repository.GetAll().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllProperties()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Complete Product",
            Description = "Full Description",
            Weight = 1.5m,
            Height = 2.5m,
            Width = 3.5m,
            Length = 4.5m
        };

        _mockDataAccess
            .Setup(d => d.Query<Product>(It.IsAny<string>(), null))
            .Returns(new[] { product });

        var result = _repository.GetAll().First();

        Assert.Equal("Complete Product", result.Name);
        Assert.Equal("Full Description", result.Description);
        Assert.Equal(1.5m, result.Weight);
        Assert.Equal(2.5m, result.Height);
        Assert.Equal(3.5m, result.Width);
        Assert.Equal(4.5m, result.Length);
    }

    [Fact]
    public void GetAll_ShouldCallQuery()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "Product 1", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m },
            new Product { Id = 2, Name = "Product 2", Weight = 2.0m, Height = 2.0m, Width = 2.0m, Length = 2.0m }
        };

        _mockDataAccess
            .Setup(d => d.Query<Product>(It.IsAny<string>(), null))
            .Returns(products);

        _repository.GetAll();

        _mockDataAccess.Verify(d => d.Query<Product>(It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public void Create_Read_Update_Integration_ShouldWork()
    {
        var product = new Product { Id = 1, Name = "Initial", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };
        
        // Create
        _repository.Create(product);
        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Once);

        // Read - setup mock ??? Execute<Product>
        _mockDataAccess
            .Setup(d => d.Execute<Product>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(product);
        
        var read = _repository.Read(product.Id);
        Assert.NotNull(read);
        Assert.Equal("Initial", read.Name);

        // Update
        product.Name = "Modified";
        _repository.Update(product);
        _mockDataAccess.Verify(d => d.Execute(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
    }

    [Fact]
    public void GetAll_ShouldReturnEmptyWhenNoProductsAdded()
    {
        _mockDataAccess
            .Setup(d => d.Query<Product>(It.IsAny<string>(), null))
            .Returns(Array.Empty<Product>());

        var result = _repository.GetAll().ToList();

        Assert.Empty(result);
        _mockDataAccess.Verify(d => d.Query<Product>(It.IsAny<string>(), null), Times.Once);
    }
}
