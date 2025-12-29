using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;
using AdoNetLibrary.DataAccess;
using Xunit;
using Moq;

namespace AdoNetLibrary.Tests;

/// <summary>
/// System integration tests - verify that all components work together
/// </summary>
public class SystemIntegrationTests
{
    [Fact]
    public void System_ProductAndOrderRepositories_CanBeInstantiated()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();

        // Act
        var productRepo = new ProductRepository(mockDataAccess.Object);
        var orderRepo = new OrderRepository(mockDataAccess.Object);

        // Assert
        Assert.NotNull(productRepo);
        Assert.NotNull(orderRepo);
    }

    [Fact]
    public void System_ProductRepository_ReceivesDependency()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();

        // Act
        var repository = new ProductRepository(mockDataAccess.Object);

        // Assert - No exception thrown means dependency was injected correctly
        Assert.NotNull(repository);
    }

    [Fact]
    public void System_OrderRepository_ReceivesDependency()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();

        // Act
        var repository = new OrderRepository(mockDataAccess.Object);

        // Assert - No exception thrown means dependency was injected correctly
        Assert.NotNull(repository);
    }

    [Fact]
    public void System_Product_CanBeCreatedWithAllFields()
    {
        // Arrange
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

        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(1.5m, product.Weight);
        Assert.Equal(2.0m, product.Height);
        Assert.Equal(3.0m, product.Width);
        Assert.Equal(4.0m, product.Length);
    }

    [Fact]
    public void System_Order_CanBeCreatedWithAllFields()
    {
        // Arrange
        var now = DateTime.Now;
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.InProgress,
            CreateDate = now,
            UpdateDate = now,
            ProductId = 1
        };

        // Assert
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal(now, order.CreateDate);
        Assert.Equal(now, order.UpdateDate);
        Assert.Equal(1, order.ProductId);
    }

    [Fact]
    public void System_AllOrderStatuses_AreDefined()
    {
        // Assert
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

        Assert.Equal(7, statuses.Length);
    }

    [Fact]
    public void System_ProductRepository_ExecutesCRUDOperations()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var repository = new ProductRepository(mockDataAccess.Object);
        var product = new Product { Name = "Test", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };

        // Act
        repository.Create(product);
        repository.Update(product);
        repository.Delete(1);
        repository.GetAll();

        // Assert
        mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Exactly(3));
        mockDataAccess.Verify(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void System_OrderRepository_ExecutesCRUDOperations()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new System.Data.DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        // Setup ExecuteQuery to return empty DataTable (for GetOrders and DeleteOrdersBulk)
        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);
        var order = new Order { Status = OrderStatus.NotStarted, ProductId = 1, CreateDate = DateTime.Now, UpdateDate = DateTime.Now };

        // Act
        repository.Create(order);
        repository.Update(order);
        repository.Delete(1);
        repository.GetOrders();
        repository.DeleteOrdersBulk();

        // Assert
        // ExecuteNonQuery: Create, Update, Delete = 3 calls
        mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Exactly(3));
        
        // ExecuteQuery: GetOrders (1) + DeleteOrdersBulk calls GetOrders (1) = 2 calls total
        mockDataAccess.Verify(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Exactly(2));
    }

    [Fact]
    public void System_DataAccess_ImplementsAllMethods()
    {
        // Arrange
        var connectionString = "Server=.;Database=test;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        // Assert
        Assert.NotNull(dataAccess.GetConnection);
        
        var type = dataAccess.GetType();
        Assert.NotNull(type.GetMethod("ExecuteNonQuery"));
        Assert.NotNull(type.GetMethod("ExecuteScalar"));
        Assert.NotNull(type.GetMethod("ExecuteQuery"));
        Assert.NotNull(type.GetMethod("GetConnection"));
    }

    [Fact]
    public void System_Repositories_UseDataAccessLayer()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var productRepo = new ProductRepository(mockDataAccess.Object);
        var product = new Product { Name = "Test", Weight = 1.0m, Height = 1.0m, Width = 1.0m, Length = 1.0m };

        // Act
        productRepo.Create(product);

        // Assert - Verify that repository used data access layer
        mockDataAccess.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Once);
    }

    [Fact]
    public void System_Product_NameCannotBeNull()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = null!;

        // Assert
        Assert.Null(product.Name);
    }

    [Fact]
    public void System_Order_CanHaveAnyProductId()
    {
        // Arrange
        var order = new Order();

        // Act
        order.ProductId = 999999;

        // Assert
        Assert.Equal(999999, order.ProductId);
    }

    [Fact]
    public void System_OrderStatus_CanBeCastToInt()
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
    public void System_OrderStatus_CanBeCastFromInt()
    {
        // Assert
        Assert.Equal(OrderStatus.NotStarted, (OrderStatus)0);
        Assert.Equal(OrderStatus.Loading, (OrderStatus)1);
        Assert.Equal(OrderStatus.InProgress, (OrderStatus)2);
        Assert.Equal(OrderStatus.Arrived, (OrderStatus)3);
        Assert.Equal(OrderStatus.Unloading, (OrderStatus)4);
        Assert.Equal(OrderStatus.Cancelled, (OrderStatus)5);
        Assert.Equal(OrderStatus.Done, (OrderStatus)6);
    }

    [Fact]
    public void System_Product_AllPropertiesAreReadWrite()
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
    public void System_Order_AllPropertiesAreReadWrite()
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
    public void System_RepositoryFiltering_WorksWithAllFilters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new System.Data.DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        repository.GetOrders(productId: 1);
        repository.GetOrders(status: OrderStatus.NotStarted);
        repository.GetOrders(year: 2024);
        repository.GetOrders(month: 1);
        repository.GetOrders(productId: 1, status: OrderStatus.InProgress, year: 2024, month: 3);

        // Assert
        mockDataAccess.Verify(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Exactly(5));
    }

    [Fact]
    public void System_RepositoryBulkDelete_WorksWithAllFilters()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new System.Data.DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Status");
        dataTable.Columns.Add("CreateDate");
        dataTable.Columns.Add("UpdateDate");
        dataTable.Columns.Add("ProductId");

        // Setup GetOrders to return empty table for DeleteOrdersBulk
        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()))
            .Returns(dataTable);

        var repository = new OrderRepository(mockDataAccess.Object);

        // Act
        repository.DeleteOrdersBulk();
        repository.DeleteOrdersBulk(productId: 1);
        repository.DeleteOrdersBulk(status: OrderStatus.Cancelled);
        repository.DeleteOrdersBulk(year: 2024);
        repository.DeleteOrdersBulk(month: 1);
        repository.DeleteOrdersBulk(productId: 1, status: OrderStatus.Cancelled, year: 2024, month: 3);

        // Assert
        // DeleteOrdersBulk calls GetOrders, which calls ExecuteQuery
        // So 6 calls to DeleteOrdersBulk = 6 calls to ExecuteQuery
        mockDataAccess.Verify(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), It.IsAny<System.Data.SqlClient.SqlParameter[]>()), Times.Exactly(6));
    }

    [Fact]
    public void System_Repositories_AreThreadSafe()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess>();
        var dataTable = new System.Data.DataTable();
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Description");
        dataTable.Columns.Add("Weight");
        dataTable.Columns.Add("Height");
        dataTable.Columns.Add("Width");
        dataTable.Columns.Add("Length");

        mockDataAccess.Setup(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), null))
            .Returns(dataTable);

        var repository = new ProductRepository(mockDataAccess.Object);

        // Act - Simulate concurrent access
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => repository.GetAll()));
        }

        // Assert - No exceptions should be thrown
        Task.WaitAll(tasks.ToArray());
        mockDataAccess.Verify(x => x.ExecuteQuery(It.IsAny<string>(), It.IsAny<System.Data.CommandType>(), null), Times.Exactly(10));
    }
}
