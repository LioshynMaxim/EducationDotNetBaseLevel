using System.Data;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using Moq;
using Xunit;

namespace AdoNetLibrary.Tests;

/// <summary>
/// Advanced unit tests for repository behavior and edge cases
/// </summary>
public class RepositoryBehaviorTests
{
    [Fact]
    public void ProductRepository_Create_PassesCorrectParameters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new ProductRepository(mockDataAccess.Object);
        
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Weight = 1.5m,
            Height = 2.0m,
            Width = 3.0m,
            Length = 4.0m
        };

        // Act
        repository.Create(product);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("INSERT")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Name") &&
                p.Any(x => x.ParameterName == "@Description") &&
                p.Any(x => x.ParameterName == "@Weight")
            )
        ), Times.Once);
    }

    [Fact]
    public void ProductRepository_Update_PassesCorrectParameters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new ProductRepository(mockDataAccess.Object);
        
        var product = new Product
        {
            Id = 1,
            Name = "Updated Product",
            Description = "Updated Description",
            Weight = 2.5m,
            Height = 3.0m,
            Width = 4.0m,
            Length = 5.0m
        };

        // Act
        repository.Update(product);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("UPDATE")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Id")
            )
        ), Times.Once);
    }

    [Fact]
    public void ProductRepository_Delete_PassesCorrectId()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new ProductRepository(mockDataAccess.Object);

        // Act
        repository.Delete(1);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Id")
            )
        ), Times.Once);
    }

    [Fact]
    public void ProductRepository_Read_ReturnsProductWithAllFields()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");
        dataTable.Rows.Add(1, "Test Product", "Test Desc", 1.5m, 2.0m, 3.0m, 4.0m);

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new ProductRepository(mockDataAccess.Object);

        // Act
        var result = repository.Read(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal("Test Desc", result.Description);
        Assert.Equal(1.5m, result.Weight);
        Assert.Equal(2.0m, result.Height);
        Assert.Equal(3.0m, result.Width);
        Assert.Equal(4.0m, result.Length);
    }

    [Fact]
    public void ProductRepository_GetAll_ReturnsMultipleProducts()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");
        
        for (int i = 1; i <= 5; i++)
        {
            dataTable.Rows.Add(i, $"Product {i}", $"Desc {i}", i * 1.0m, i * 2.0m, i * 3.0m, i * 4.0m);
        }

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), null))
            .Returns(dataTable);

        var repository = new ProductRepository(mockDataAccess.Object);

        // Act
        var result = repository.GetAll().ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(i + 1, result[i].Id);
        }
    }

    [Fact]
    public void ProductRepository_GetAll_HandlesEmptyResult()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), null))
            .Returns(dataTable);

        var repository = new ProductRepository(mockDataAccess.Object);

        // Act
        var result = repository.GetAll().ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void OrderRepository_Create_PassesCorrectParameters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new OrderRepository(mockDataAccess.Object);
        
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        // Act
        repository.Create(order);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("INSERT")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Status") &&
                p.Any(x => x.ParameterName == "@ProductId")
            )
        ), Times.Once);
    }

    [Fact]
    public void OrderRepository_Update_PassesCorrectParameters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new OrderRepository(mockDataAccess.Object);
        
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        // Act
        repository.Update(order);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("UPDATE")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Status")
            )
        ), Times.Once);
    }

    [Fact]
    public void OrderRepository_Delete_PassesCorrectId()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        repository.Delete(1);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            CommandType.Text,
            It.Is<System.Data.SqlClient.SqlParameter[]>(p => 
                p.Any(x => x.ParameterName == "@Id")
            )
        ), Times.Once);
    }

    [Fact]
    public void OrderRepository_GetOrders_UsesLinqFiltering()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        repository.GetOrders(productId: 1, status: OrderStatus.NotStarted, year: 2024, month: 1);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteQuery(
            It.Is<string>(sql => sql.Contains("SELECT") && !sql.Contains("sp_")),
            CommandType.Text,
            It.IsAny<System.Data.SqlClient.SqlParameter[]>()
        ), Times.Once);
    }

    [Fact]
    public void OrderRepository_GetOrders_WithoutFilters_ReturnsAll()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 2);

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.GetOrders().ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void OrderRepository_DeleteOrdersBulk_UsesLinqAndDeletes()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 5, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 5, DateTime.Now, DateTime.Now, 1);

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.DeleteOrdersBulk(status: OrderStatus.Cancelled);

        // Assert
        Assert.Equal(2, result);
        mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            CommandType.Text,
            It.IsAny<System.Data.SqlClient.SqlParameter[]>()
        ), Times.Exactly(2));
    }

    [Fact]
    public void OrderRepository_DeleteOrdersBulk_ReturnsCorrectCount()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.DeleteOrdersBulk(status: OrderStatus.Done, year: 1900);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void OrderRepository_GetOrders_FiltersByProductId()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 2);

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.GetOrders(productId: 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().ProductId);
    }

    [Fact]
    public void OrderRepository_Read_ReturnsOrderWithAllFields()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        
        var now = DateTime.Now;
        dataTable.Rows.Add(1, 2, now, now, 5);

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.Read(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(OrderStatus.InProgress, result.Status);
        Assert.Equal(5, result.ProductId);
    }

    [Fact]
    public void ProductRepository_Read_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new ProductRepository(mockDataAccess.Object);

        // Act
        var result = repository.Read(999999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void OrderRepository_Read_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.Read(999999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void OrderRepository_GetOrders_ReturnsMultipleOrdersWithCorrectStatuses()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        
        var now = DateTime.Now;
        for (int i = 0; i < 7; i++)
        {
            dataTable.Rows.Add(i + 1, i, now, now, 1);
        }

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        var result = repository.GetOrders().ToList();

        // Assert
        Assert.Equal(7, result.Count);
        Assert.True(result.Any(o => o.Status == OrderStatus.NotStarted));
        Assert.True(result.Any(o => o.Status == OrderStatus.Loading));
        Assert.True(result.Any(o => o.Status == OrderStatus.InProgress));
        Assert.True(result.Any(o => o.Status == OrderStatus.Arrived));
        Assert.True(result.Any(o => o.Status == OrderStatus.Unloading));
        Assert.True(result.Any(o => o.Status == OrderStatus.Cancelled));
        Assert.True(result.Any(o => o.Status == OrderStatus.Done));
    }

    [Fact]
    public void DataAccess_GetConnection_ReturnsValidConnection()
    {
        // Arrange
        var connectionString = "Server=.;Database=test;Trusted_Connection=true;";
        var dataAccess = new AdoNetLibrary.DataAccess.DataAccess(connectionString);

        // Act
        var connection = dataAccess.GetConnection();

        // Assert
        Assert.NotNull(connection);
        Assert.IsType<System.Data.SqlClient.SqlConnection>(connection);
    }
}
