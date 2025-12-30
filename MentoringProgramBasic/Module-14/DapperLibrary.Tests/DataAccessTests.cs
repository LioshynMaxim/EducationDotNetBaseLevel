using DapperLibrary.DataAccess;
using Xunit;

namespace DapperLibrary.Tests;

public class DataAccessTests
{
    [Fact]
    public void Constructor_WithValidConnectionString_ShouldCreateInstance()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";

        var dataAccess = new DataAccess.DataAccess(connectionString);

        Assert.NotNull(dataAccess);
        Assert.Equal(connectionString, dataAccess.ConnectionString);
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DataAccess.DataAccess(null!));
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DataAccess.DataAccess(string.Empty));
    }

    [Fact]
    public void ConnectionString_ShouldReturnCorrectValue()
    {
        var connectionString = "Server=localhost;Database=MyDb;User Id=sa;Password=password;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        Assert.Equal(connectionString, dataAccess.ConnectionString);
    }

    [Fact]
    public void GetConnection_ShouldReturnValidConnection()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var connection = dataAccess.GetConnection();

        Assert.NotNull(connection);
        Assert.Equal(System.Data.ConnectionState.Closed, connection.State);
    }

    [Fact]
    public void GetConnection_ShouldHaveCorrectConnectionString()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var connection = dataAccess.GetConnection();

        Assert.Equal(connectionString, connection.ConnectionString);
    }

    [Fact]
    public void GetConnection_MultipleCalls_ShouldReturnNewInstances()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var connection1 = dataAccess.GetConnection();
        var connection2 = dataAccess.GetConnection();

        Assert.NotSame(connection1, connection2);
        Assert.Equal(connection1.ConnectionString, connection2.ConnectionString);
    }

    [Fact]
    public void Execute_ShouldBeCallable()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var exception = Record.Exception(() => dataAccess.Execute("SELECT 1", null));
        
        Assert.NotNull(exception);
    }

    [Fact]
    public void Query_ShouldBeCallable()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var exception = Record.Exception(() => dataAccess.Query<System.Text.StringBuilder>("SELECT 1", null));
        
        Assert.NotNull(exception);
    }

    [Fact]
    public void Constructor_WithDifferentConnectionStrings_ShouldStoreCorrectly()
    {
        var cs1 = "Server=server1;Database=db1;";
        var cs2 = "Server=server2;Database=db2;";

        var da1 = new DataAccess.DataAccess(cs1);
        var da2 = new DataAccess.DataAccess(cs2);

        Assert.Equal(cs1, da1.ConnectionString);
        Assert.Equal(cs2, da2.ConnectionString);
        Assert.NotEqual(da1.ConnectionString, da2.ConnectionString);
    }

    [Fact]
    public void GetConnection_ConnectionType_ShouldBeSqlConnection()
    {
        var connectionString = "Server=.;Database=TestDb;Trusted_Connection=true;";
        var dataAccess = new DataAccess.DataAccess(connectionString);

        var connection = dataAccess.GetConnection();

        Assert.IsAssignableFrom<Microsoft.Data.SqlClient.SqlConnection>(connection);
    }
}
