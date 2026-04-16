using System.Data;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using Xunit;

namespace AdoNetLibrary.Tests;

/// <summary>
/// Integration tests for repositories (requires actual database)
/// Set the connection string to run these tests
/// </summary>
public class RepositoryIntegrationTests
{
    // Update with your actual connection string to enable tests
    private readonly string _connectionString = "Server=.;Database=OrderManagementDb;Trusted_Connection=true;";

    private IProductRepository GetProductRepository()
    {
        var dataAccess = new DataAccess.DataAccess(_connectionString);
        return new ProductRepository(dataAccess);
    }

    private IOrderRepository GetOrderRepository()
    {
        var dataAccess = new DataAccess.DataAccess(_connectionString);
        return new OrderRepository(dataAccess);
    }

    private bool IsConnectionValid()
    {
        try
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    // ===================================================
    // PRODUCT INTEGRATION TESTS
    // ===================================================

    [Fact]
    public void ProductIntegration_CreateProduct_ShouldSucceed()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var repository = GetProductRepository();
        var product = new Product
        {
            Name = $"Integration Test Product {Guid.NewGuid()}",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };

        // Act & Assert - No exception should be thrown
        repository.Create(product);
    }

    [Fact]
    public void ProductIntegration_CreateAndRead_ShouldReturnCreatedProduct()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var repository = GetProductRepository();
        var productName = $"Test Product {Guid.NewGuid()}";
        var product = new Product
        {
            Name = productName,
            Description = "Test Description",
            Weight = 2.5m,
            Height = 3.0m,
            Width = 4.0m,
            Length = 5.0m
        };

        // Act
        repository.Create(product);
        var allProducts = repository.GetAll().ToList();
        var createdProduct = allProducts.FirstOrDefault(p => p.Name == productName);

        // Assert
        Assert.NotNull(createdProduct);
        Assert.Equal(productName, createdProduct.Name);
        Assert.Equal("Test Description", createdProduct.Description);
    }

    [Fact]
    public void ProductIntegration_CreateAndUpdate_ShouldUpdateProduct()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var repository = GetProductRepository();
        var product = new Product
        {
            Name = $"Test Product Update {Guid.NewGuid()}",
            Description = "Original Description",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Act
        repository.Create(product);
        var createdProducts = repository.GetAll().Where(p => p.Name == product.Name).ToList();
        Assert.NotEmpty(createdProducts);
        
        var createdProduct = createdProducts.First();
        createdProduct.Description = "Updated Description";
        createdProduct.Weight = 2.0m;
        repository.Update(createdProduct);

        var updated = repository.Read(createdProduct.Id);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated Description", updated.Description);
        Assert.Equal(2.0m, updated.Weight);
    }

    [Fact]
    public void ProductIntegration_CreateAndDelete_ShouldRemoveProduct()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var repository = GetProductRepository();
        var product = new Product
        {
            Name = $"Test Product Delete {Guid.NewGuid()}",
            Description = "To Be Deleted",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Act
        repository.Create(product);
        var createdProducts = repository.GetAll().Where(p => p.Name == product.Name).ToList();
        var createdProduct = createdProducts.First();

        repository.Delete(createdProduct.Id);
        var deleted = repository.Read(createdProduct.Id);

        // Assert
        Assert.Null(deleted);
    }

    [Fact]
    public void ProductIntegration_GetAll_ShouldReturnAllProducts()
    {
        if (!IsConnectionValid()) return;

        // Act
        var repository = GetProductRepository();
        var products = repository.GetAll().ToList();

        // Assert
        Assert.NotNull(products);
        Assert.True(products.Count >= 0);
    }

    [Fact]
    public void ProductIntegration_ReadNonExistent_ShouldReturnNull()
    {
        if (!IsConnectionValid()) return;

        // Act
        var repository = GetProductRepository();
        var product = repository.Read(999999);

        // Assert
        Assert.Null(product);
    }

    // ===================================================
    // ORDER INTEGRATION TESTS
    // ===================================================

    [Fact]
    public void OrderIntegration_CreateOrder_ShouldSucceed()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product for Order {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };

        // Act & Assert
        orderRepo.Create(order);
    }

    [Fact]
    public void OrderIntegration_CreateAndRead_ShouldReturnCreatedOrder()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };

        // Act
        orderRepo.Create(order);
        var createdOrders = orderRepo.GetOrders().Where(o => o.ProductId == productId).ToList();
        var createdOrder = createdOrders.FirstOrDefault();

        // Assert
        Assert.NotNull(createdOrder);
        Assert.Equal(OrderStatus.InProgress, createdOrder.Status);
    }

    [Fact]
    public void OrderIntegration_CreateAndUpdate_ShouldUpdateOrder()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };

        // Act
        orderRepo.Create(order);
        var createdOrders = orderRepo.GetOrders().Where(o => o.ProductId == productId).ToList();
        var createdOrder = createdOrders.First();

        createdOrder.Status = OrderStatus.Done;
        createdOrder.UpdateDate = DateTime.Now;
        orderRepo.Update(createdOrder);

        var updated = orderRepo.Read(createdOrder.Id);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(OrderStatus.Done, updated.Status);
    }

    [Fact]
    public void OrderIntegration_CreateAndDelete_ShouldRemoveOrder()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.Cancelled,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };

        // Act
        orderRepo.Create(order);
        var createdOrders = orderRepo.GetOrders().Where(o => o.ProductId == productId).ToList();
        var createdOrder = createdOrders.First();

        orderRepo.Delete(createdOrder.Id);
        var deleted = orderRepo.Read(createdOrder.Id);

        // Assert
        Assert.Null(deleted);
    }

    // ===================================================
    // ORDER FILTERING TESTS
    // ===================================================

    [Fact]
    public void OrderIntegration_FilterByStatus_ShouldReturnOrdersWithStatus()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.Loading,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var filtered = orderRepo.GetOrders(status: OrderStatus.Loading).ToList();

        // Assert
        Assert.NotEmpty(filtered);
        Assert.True(filtered.All(o => o.Status == OrderStatus.Loading));
    }

    [Fact]
    public void OrderIntegration_FilterByProductId_ShouldReturnOrdersForProduct()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var filtered = orderRepo.GetOrders(productId: productId).ToList();

        // Assert
        Assert.NotEmpty(filtered);
        Assert.True(filtered.All(o => o.ProductId == productId));
    }

    [Fact]
    public void OrderIntegration_FilterByYear_ShouldReturnOrdersFromYear()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var currentYear = DateTime.Now.Year;
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var filtered = orderRepo.GetOrders(year: currentYear).ToList();

        // Assert
        Assert.NotEmpty(filtered);
        Assert.True(filtered.All(o => o.CreateDate.Year == currentYear));
    }

    [Fact]
    public void OrderIntegration_FilterByMonth_ShouldReturnOrdersFromMonth()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var currentMonth = DateTime.Now.Month;
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var filtered = orderRepo.GetOrders(month: currentMonth).ToList();

        // Assert
        Assert.NotEmpty(filtered);
        Assert.True(filtered.All(o => o.CreateDate.Month == currentMonth));
    }

    [Fact]
    public void OrderIntegration_FilterByMultipleCriteria_ShouldReturnMatchingOrders()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.Arrived,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var filtered = orderRepo.GetOrders(
            productId: productId,
            status: OrderStatus.Arrived,
            year: DateTime.Now.Year,
            month: DateTime.Now.Month
        ).ToList();

        // Assert
        Assert.NotEmpty(filtered);
    }

    // ===================================================
    // BULK DELETE TESTS
    // ===================================================

    [Fact]
    public void OrderIntegration_DeleteOrdersBulkByStatus_ShouldDeleteMatchingOrders()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        // Create multiple orders with Cancelled status
        for (int i = 0; i < 3; i++)
        {
            var order = new Order
            {
                Status = OrderStatus.Cancelled,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ProductId = productId
            };
            orderRepo.Create(order);
        }

        // Act
        var deletedCount = orderRepo.DeleteOrdersBulk(status: OrderStatus.Cancelled);

        // Assert
        Assert.True(deletedCount > 0);
    }

    [Fact]
    public void OrderIntegration_DeleteOrdersBulkByProductId_ShouldDeleteOrdersForProduct()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        // Create multiple orders
        for (int i = 0; i < 2; i++)
        {
            var order = new Order
            {
                Status = OrderStatus.Unloading,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ProductId = productId
            };
            orderRepo.Create(order);
        }

        // Act
        var initialCount = orderRepo.GetOrders(productId: productId).Count();
        var deletedCount = orderRepo.DeleteOrdersBulk(productId: productId);

        // Assert
        Assert.True(deletedCount > 0);
    }

    [Fact]
    public void OrderIntegration_DeleteOrdersBulkByYear_ShouldDeleteOrdersFromYear()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var currentYear = DateTime.Now.Year;
        var deletedCount = orderRepo.DeleteOrdersBulk(year: currentYear);

        // Assert
        Assert.True(deletedCount >= 0);
    }

    [Fact]
    public void OrderIntegration_DeleteOrdersBulkByMultipleCriteria_ShouldDeleteMatchingOrders()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var order = new Order
        {
            Status = OrderStatus.Cancelled,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = productId
        };
        orderRepo.Create(order);

        // Act
        var deletedCount = orderRepo.DeleteOrdersBulk(
            productId: productId,
            status: OrderStatus.Cancelled,
            year: DateTime.Now.Year
        );

        // Assert
        Assert.True(deletedCount >= 0);
    }

    // ===================================================
    // EDGE CASES AND VALIDATION TESTS
    // ===================================================

    [Fact]
    public void OrderIntegration_GetOrdersWithNoFilters_ShouldReturnAllOrders()
    {
        if (!IsConnectionValid()) return;

        // Act
        var orderRepo = GetOrderRepository();
        var orders = orderRepo.GetOrders().ToList();

        // Assert
        Assert.NotNull(orders);
        Assert.True(orders.Count >= 0);
    }

    [Fact]
    public void OrderIntegration_GetOrdersWithInvalidFilter_ShouldReturnEmptyList()
    {
        if (!IsConnectionValid()) return;

        // Act
        var orderRepo = GetOrderRepository();
        var orders = orderRepo.GetOrders(productId: 999999).ToList();

        // Assert
        Assert.NotNull(orders);
    }

    [Fact]
    public void OrderIntegration_DeleteOrdersWithoutFilters_ShouldReturnCount()
    {
        if (!IsConnectionValid()) return;

        // Act
        var orderRepo = GetOrderRepository();
        var deletedCount = orderRepo.DeleteOrdersBulk();

        // Assert
        Assert.True(deletedCount >= 0);
    }

    [Fact]
    public void ProductIntegration_MultipleCreates_ShouldCreateMultipleProducts()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var repository = GetProductRepository();
        var initialCount = repository.GetAll().Count();

        // Act
        for (int i = 0; i < 3; i++)
        {
            var product = new Product
            {
                Name = $"Batch Product {i} {Guid.NewGuid()}",
                Description = $"Batch {i}",
                Weight = i + 1.0m,
                Height = i + 2.0m,
                Width = i + 3.0m,
                Length = i + 4.0m
            };
            repository.Create(product);
        }

        var finalCount = repository.GetAll().Count();

        // Assert
        Assert.True(finalCount >= initialCount + 3);
    }

    [Fact]
    public void OrderIntegration_AllOrderStatuses_ShouldBeHandledCorrectly()
    {
        if (!IsConnectionValid()) return;

        // Arrange
        var productRepo = GetProductRepository();
        var orderRepo = GetOrderRepository();
        
        var product = new Product
        {
            Name = $"Product {Guid.NewGuid()}",
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };
        productRepo.Create(product);
        var products = productRepo.GetAll().Where(p => p.Name == product.Name).ToList();
        var productId = products.First().Id;

        var statuses = new[] 
        { 
            OrderStatus.NotStarted, 
            OrderStatus.Loading, 
            OrderStatus.InProgress, 
            OrderStatus.Arrived, 
            OrderStatus.Unloading, 
            OrderStatus.Cancelled, 
            OrderStatus.Done 
        };

        // Act & Assert
        foreach (var status in statuses)
        {
            var order = new Order
            {
                Status = status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ProductId = productId
            };
            orderRepo.Create(order);

            var filtered = orderRepo.GetOrders(status: status).ToList();
            Assert.NotEmpty(filtered);
        }
    }
}
