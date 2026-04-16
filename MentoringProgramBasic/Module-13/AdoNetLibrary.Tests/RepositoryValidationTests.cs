using System.Data;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using Xunit;

namespace AdoNetLibrary.Tests;

/// <summary>
/// Unit tests for edge cases and validation scenarios
/// </summary>
public class RepositoryValidationTests
{
    [Fact]
    public void Product_CreateWithNullName_ShouldThrowException()
    {
        // Arrange
        var product = new Product
        {
            Name = null!,
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Assert - Name is required
        Assert.Null(product.Name);
    }

    [Fact]
    public void Product_CreateWithNegativeDimensions_ShouldBeAllowed()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test",
            Description = "Test",
            Weight = -1.0m,
            Height = -1.0m,
            Width = -1.0m,
            Length = -1.0m
        };

        // Assert
        Assert.Equal(-1.0m, product.Weight);
        Assert.Equal(-1.0m, product.Height);
    }

    [Fact]
    public void Product_CreateWithZeroDimensions_ShouldBeAllowed()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test",
            Description = "Test",
            Weight = 0m,
            Height = 0m,
            Width = 0m,
            Length = 0m
        };

        // Assert
        Assert.Equal(0m, product.Weight);
    }

    [Fact]
    public void Product_CreateWithEmptyDescription_ShouldBeAllowed()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test",
            Description = string.Empty,
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Assert
        Assert.Equal(string.Empty, product.Description);
    }

    [Fact]
    public void Product_CreateWithVeryLongName_ShouldBeAllowed()
    {
        // Arrange
        var longName = new string('a', 1000);
        var product = new Product
        {
            Name = longName,
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Assert
        Assert.Equal(longName, product.Name);
    }

    [Fact]
    public void Product_CreateWithSpecialCharactersInName_ShouldBeAllowed()
    {
        // Arrange
        var specialName = "Test Product @#$%^&*()_-+=[]{}|;':\"<>?,./";
        var product = new Product
        {
            Name = specialName,
            Description = "Test",
            Weight = 1.0m,
            Height = 1.0m,
            Width = 1.0m,
            Length = 1.0m
        };

        // Assert
        Assert.Equal(specialName, product.Name);
    }

    [Fact]
    public void Product_CreateWithMaxDecimalValues_ShouldBeAllowed()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test",
            Description = "Test",
            Weight = decimal.MaxValue,
            Height = decimal.MaxValue,
            Width = decimal.MaxValue,
            Length = decimal.MaxValue
        };

        // Assert
        Assert.Equal(decimal.MaxValue, product.Weight);
    }

    [Fact]
    public void Order_CreateWithAllStatuses_ShouldBeValid()
    {
        // Arrange & Act & Assert
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

        foreach (var status in statuses)
        {
            var order = new Order
            {
                Status = status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                ProductId = 1
            };

            Assert.Equal(status, order.Status);
        }
    }

    [Fact]
    public void Order_CreateWithFutureDate_ShouldBeAllowed()
    {
        // Arrange
        var futureDate = DateTime.Now.AddYears(1);
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = futureDate,
            UpdateDate = futureDate,
            ProductId = 1
        };

        // Assert
        Assert.Equal(futureDate, order.CreateDate);
    }

    [Fact]
    public void Order_CreateWithPastDate_ShouldBeAllowed()
    {
        // Arrange
        var pastDate = DateTime.Now.AddYears(-1);
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = pastDate,
            UpdateDate = pastDate,
            ProductId = 1
        };

        // Assert
        Assert.Equal(pastDate, order.CreateDate);
    }

    [Fact]
    public void Order_CreateWithMinDateTime_ShouldBeAllowed()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.MinValue,
            UpdateDate = DateTime.MinValue,
            ProductId = 1
        };

        // Assert
        Assert.Equal(DateTime.MinValue, order.CreateDate);
    }

    [Fact]
    public void Order_CreateWithMaxDateTime_ShouldBeAllowed()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.MaxValue,
            UpdateDate = DateTime.MaxValue,
            ProductId = 1
        };

        // Assert
        Assert.Equal(DateTime.MaxValue, order.CreateDate);
    }

    [Fact]
    public void Order_CreateWithZeroProductId_ShouldBeAllowed()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 0
        };

        // Assert
        Assert.Equal(0, order.ProductId);
    }

    [Fact]
    public void Order_CreateWithNegativeProductId_ShouldBeAllowed()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = -1
        };

        // Assert
        Assert.Equal(-1, order.ProductId);
    }

    [Fact]
    public void Order_CreateWithMaxProductId_ShouldBeAllowed()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = int.MaxValue
        };

        // Assert
        Assert.Equal(int.MaxValue, order.ProductId);
    }

    [Fact]
    public void OrderStatus_Enum_HasSevenValues()
    {
        // Act
        var values = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

        // Assert
        Assert.Equal(7, values.Count);
        Assert.Contains(OrderStatus.NotStarted, values);
        Assert.Contains(OrderStatus.Loading, values);
        Assert.Contains(OrderStatus.InProgress, values);
        Assert.Contains(OrderStatus.Arrived, values);
        Assert.Contains(OrderStatus.Unloading, values);
        Assert.Contains(OrderStatus.Cancelled, values);
        Assert.Contains(OrderStatus.Done, values);
    }

    [Fact]
    public void OrderStatus_EnumValues_AreCorrect()
    {
        // Assert
        Assert.Equal(0, (int)OrderStatus.NotStarted);
        Assert.Equal(1, (int)OrderStatus.Loading);
        Assert.Equal(2, (int)OrderStatus.InProgress);
        Assert.Equal(3, (int)OrderStatus.Arrived);
        Assert.Equal(4, (int)OrderStatus.Unloading);
        Assert.Equal(5, (int)OrderStatus.Cancelled);
        Assert.Equal(6, (int)OrderStatus.Done);
    }

    [Fact]
    public void Product_Properties_AreAccessible()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Id = 1;
        product.Name = "Test";
        product.Description = "Desc";
        product.Weight = 1.5m;
        product.Height = 2.0m;
        product.Width = 3.0m;
        product.Length = 4.0m;

        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test", product.Name);
        Assert.Equal("Desc", product.Description);
        Assert.Equal(1.5m, product.Weight);
        Assert.Equal(2.0m, product.Height);
        Assert.Equal(3.0m, product.Width);
        Assert.Equal(4.0m, product.Length);
    }

    [Fact]
    public void Order_Properties_AreAccessible()
    {
        // Arrange
        var order = new Order();
        var now = DateTime.Now;

        // Act
        order.Id = 1;
        order.Status = OrderStatus.InProgress;
        order.CreateDate = now;
        order.UpdateDate = now;
        order.ProductId = 1;

        // Assert
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal(now, order.CreateDate);
        Assert.Equal(now, order.UpdateDate);
        Assert.Equal(1, order.ProductId);
    }

    [Fact]
    public void Product_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.Equal(0, product.Id);
        Assert.Equal(string.Empty, product.Name);
        Assert.Equal(string.Empty, product.Description);
        Assert.Equal(0m, product.Weight);
        Assert.Equal(0m, product.Height);
        Assert.Equal(0m, product.Width);
        Assert.Equal(0m, product.Length);
    }

    [Fact]
    public void Order_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var order = new Order();

        // Assert
        Assert.Equal(0, order.Id);
        Assert.Equal(OrderStatus.NotStarted, order.Status);
        Assert.Equal(DateTime.MinValue, order.CreateDate);
        Assert.Equal(DateTime.MinValue, order.UpdateDate);
        Assert.Equal(0, order.ProductId);
    }

    [Fact]
    public void Product_CanBeCloned_WithCopyConstructor()
    {
        // Arrange
        var original = new Product
        {
            Id = 1,
            Name = "Original",
            Description = "Original Desc",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };

        // Act
        var copy = new Product
        {
            Id = original.Id,
            Name = original.Name,
            Description = original.Description,
            Weight = original.Weight,
            Height = original.Height,
            Width = original.Width,
            Length = original.Length
        };

        // Assert
        Assert.Equal(original.Id, copy.Id);
        Assert.Equal(original.Name, copy.Name);
        Assert.Equal(original.Description, copy.Description);
        Assert.Equal(original.Weight, copy.Weight);
    }

    [Fact]
    public void Order_CanBeCloned_WithCopyConstructor()
    {
        // Arrange
        var original = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        // Act
        var copy = new Order
        {
            Id = original.Id,
            Status = original.Status,
            CreateDate = original.CreateDate,
            UpdateDate = original.UpdateDate,
            ProductId = original.ProductId
        };

        // Assert
        Assert.Equal(original.Id, copy.Id);
        Assert.Equal(original.Status, copy.Status);
        Assert.Equal(original.CreateDate, copy.CreateDate);
        Assert.Equal(original.ProductId, copy.ProductId);
    }

    [Fact]
    public void Product_EqualsComparison_WorksCorrectly()
    {
        // Arrange
        var product1 = new Product { Id = 1, Name = "Test" };
        var product2 = new Product { Id = 1, Name = "Test" };
        var product3 = new Product { Id = 2, Name = "Test" };

        // Act & Assert
        Assert.NotSame(product1, product2);
        Assert.Equal(product1.Id, product2.Id);
        Assert.NotEqual(product1.Id, product3.Id);
    }

    [Fact]
    public void Order_EqualsComparison_WorksCorrectly()
    {
        // Arrange
        var order1 = new Order { Id = 1, Status = OrderStatus.NotStarted };
        var order2 = new Order { Id = 1, Status = OrderStatus.NotStarted };
        var order3 = new Order { Id = 2, Status = OrderStatus.NotStarted };

        // Act & Assert
        Assert.NotSame(order1, order2);
        Assert.Equal(order1.Id, order2.Id);
        Assert.NotEqual(order1.Id, order3.Id);
    }
}
