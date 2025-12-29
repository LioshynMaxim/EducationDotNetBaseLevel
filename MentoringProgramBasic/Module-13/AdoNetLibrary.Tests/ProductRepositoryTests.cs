using System.Data;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using Moq;
using Xunit;

namespace AdoNetLibrary.Tests;

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
    public void Create_ShouldCallExecuteNonQuery()
    {
        // Arrange
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
        _repository.Create(product);

        // Assert
        _mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void Read_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1;
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");
        dataTable.Rows.Add(1, "Test Product", "Test Description", 1.5m, 2.0m, 3.0m, 4.0m);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.Read(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public void Read_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var dataTable = new DataTable();
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
        _repository.Update(product);

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
    public void GetAll_ShouldReturnAllProducts()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");
        dataTable.Rows.Add(1, "Product 1", "Description 1", 1.5m, 2.0m, 3.0m, 4.0m);
        dataTable.Rows.Add(2, "Product 2", "Description 2", 2.5m, 3.0m, 4.0m, 5.0m);

        _mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }
}
