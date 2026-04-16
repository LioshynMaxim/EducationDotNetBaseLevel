using System.Data;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using Moq;
using Xunit;

namespace AdoNetLibrary.Tests;

public class OrderRepositoryTests
{
    private readonly Mock<IDataAccess> _mockDataAccess;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        _mockDataAccess = new Mock<IDataAccess>();
        _repository = new OrderRepository(_mockDataAccess.Object);
    }

    [Fact]
    public void Create_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        // Act
        _repository.Create(order);

        // Assert
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = 1;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.Read(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(OrderStatus.NotStarted, result.Status);
        Assert.Equal(1, result.ProductId);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.Read(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Update_ShouldCallExecuteNonQuery()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            ProductId = 1
        };

        // Act
        _repository.Update(order);

        // Assert
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void Delete_ShouldCallExecuteNonQuery()
    {
        // Act
        _repository.Delete(1);

        // Assert
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void GetOrders_WithoutFilters_ShouldReturnAllOrders()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders().ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetOrders_WithProductFilter_ShouldReturnFilteredOrders()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 2);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders(productId: 1).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().ProductId);
    }

    [Fact]
    public void GetOrders_WithStatusFilter_ShouldReturnFilteredOrders()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 2, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders(status: OrderStatus.InProgress).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal(OrderStatus.InProgress, result.First().Status);
    }

    [Fact]
    public void GetOrders_WithYearFilter_ShouldReturnFilteredOrders()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, new DateTime(currentYear, 1, 1), DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, new DateTime(currentYear - 1, 1, 1), DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders(year: currentYear).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(currentYear, result.First().CreateDate.Year);
    }

    [Fact]
    public void GetOrders_WithMonthFilter_ShouldReturnFilteredOrders()
    {
        // Arrange
        var now = DateTime.Now;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, new DateTime(now.Year, 1, 1), DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, new DateTime(now.Year, 2, 1), DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders(month: 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().CreateDate.Month);
    }

    [Fact]
    public void GetOrders_WithMultipleFilters_ShouldReturnFilteredOrders()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 2, new DateTime(currentYear, 1, 1), DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, new DateTime(currentYear, 2, 1), DateTime.Now, 2);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetOrders(productId: 1, status: OrderStatus.InProgress, year: currentYear, month: 1).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void DeleteOrdersBulk_WithStatus_ShouldDeleteAndReturnCount()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 5, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 5, DateTime.Now, DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.DeleteOrdersBulk(status: OrderStatus.Cancelled);

        // Assert
        Assert.Equal(2, result);
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            It.IsAny<CommandType>(),
            It.IsAny<System.Data.SqlClient.SqlParameter[]>()
        ), Times.Exactly(2));
    }

    [Fact]
    public void DeleteOrdersBulk_WithMultipleFilters_ShouldReturnDeletedCount()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, new DateTime(currentYear, 1, 1), DateTime.Now, 1);
        dataTable.Rows.Add(2, 0, new DateTime(currentYear, 2, 1), DateTime.Now, 1);
        dataTable.Rows.Add(3, 0, new DateTime(currentYear - 1, 1, 1), DateTime.Now, 1);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.DeleteOrdersBulk(productId: 1, status: OrderStatus.NotStarted, year: currentYear);

        // Assert
        Assert.Equal(2, result);
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            It.IsAny<CommandType>(),
            It.IsAny<System.Data.SqlClient.SqlParameter[]>()
        ), Times.Exactly(2));
    }

    [Fact]
    public void DeleteOrdersBulk_WithNoMatches_ShouldReturnZero()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.DeleteOrdersBulk(status: OrderStatus.Done, year: 1900);

        // Assert
        Assert.Equal(0, result);
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(
            It.Is<string>(sql => sql.Contains("DELETE")),
            It.IsAny<CommandType>(),
            It.IsAny<System.Data.SqlClient.SqlParameter[]>()
        ), Times.Never);
    }

    [Fact]
    public void GetAll_ShouldReturnAllOrders()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");
        dataTable.Rows.Add(1, 0, DateTime.Now, DateTime.Now, 1);
        dataTable.Rows.Add(2, 1, DateTime.Now, DateTime.Now, 2);
        dataTable.Rows.Add(3, 2, DateTime.Now, DateTime.Now, 3);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count);
    }
}
