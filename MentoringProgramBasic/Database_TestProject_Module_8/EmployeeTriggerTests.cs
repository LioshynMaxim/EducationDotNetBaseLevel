using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

namespace Database_TestProject_Module_8;

[TestClass]
public class EmployeeTriggerTests
{
    private const string ConnectionString = "Data Source=:memory:";
    private IDbConnection? _connection;

    [TestInitialize]
    public void Setup()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
        CreateTables();
        CreateTrigger();
    }

    [TestCleanup]
    public void TearDown()
    {
        _connection?.Dispose();
    }

    private void CreateTables()
    {
        _connection!.Execute(@"
            CREATE TABLE Addresses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Street TEXT NOT NULL,
                City TEXT,
                State TEXT,
                ZipCode TEXT
            )");

        _connection.Execute(@"
            CREATE TABLE Persons (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL
            )");

        _connection.Execute(@"
            CREATE TABLE Companies (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                AddressId INTEGER NOT NULL,
                FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
            )");

        _connection.Execute(@"
            CREATE TABLE Employees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AddressId INTEGER NOT NULL,
                PersonId INTEGER NOT NULL,
                CompanyName TEXT NOT NULL,
                Position TEXT,
                EmployeeName TEXT,
                FOREIGN KEY (AddressId) REFERENCES Addresses(Id),
                FOREIGN KEY (PersonId) REFERENCES Persons(Id)
            )");
    }

    private void CreateTrigger()
    {
        _connection!.Execute(@"
            CREATE TRIGGER EmployeeTrigger
            AFTER INSERT ON Employees
            BEGIN
                INSERT INTO Companies (Name, AddressId)
                SELECT DISTINCT NEW.CompanyName, NEW.AddressId
                FROM (SELECT NEW.CompanyName AS CompanyName, NEW.AddressId AS AddressId) AS inserted
                WHERE NOT EXISTS (
                    SELECT 1 FROM Companies c
                    WHERE c.Name = NEW.CompanyName AND c.AddressId = NEW.AddressId
                );
            END");
    }

    [TestMethod]
    public void EmployeeTrigger_WhenEmployeeInserted_ShouldCreateCompanyWithSameAddress()
    {
        // Arrange
        var addressId = InsertAddress("123 Main St", "New York", "NY", "10001");
        var personId = InsertPerson("John", "Doe");

        // Act
        InsertEmployee(addressId, personId, "TechCorp", "Developer", "John Doe");

        // Assert
        var company = _connection!.QuerySingleOrDefault<Company>(
            @"SELECT Id, Name, AddressId 
              FROM Companies 
              WHERE Name = @CompanyName",
            new { CompanyName = "TechCorp" });

        Assert.IsNotNull(company, "Company should be created by trigger");
        Assert.AreEqual("TechCorp", company.Name);
        Assert.AreEqual(addressId, company.AddressId);
    }

    [TestMethod]
    public void EmployeeTrigger_WhenCompanyAlreadyExists_ShouldNotCreateDuplicate()
    {
        // Arrange
        var addressId = InsertAddress("123 Main St", "New York", "NY", "10001");
        var personId1 = InsertPerson("John", "Doe");
        var personId2 = InsertPerson("Jane", "Smith");

        // Act
        InsertEmployee(addressId, personId1, "TechCorp", "Developer", "John Doe");

        // Act
        InsertEmployee(addressId, personId2, "TechCorp", "Manager", "Jane Smith");

        // Assert
        var companyCount = _connection!.ExecuteScalar<int>(
            @"SELECT COUNT(*) 
              FROM Companies 
              WHERE Name = @CompanyName AND AddressId = @AddressId",
            new { CompanyName = "TechCorp", AddressId = addressId });

        Assert.AreEqual(1, companyCount, "Should not create duplicate company");
    }

    [TestMethod]
    public void EmployeeTrigger_WithDifferentAddresses_ShouldCreateSeparateCompanies()
    {
        // Arrange
        var addressId1 = InsertAddress("123 Main St", "New York", "NY", "10001");
        var addressId2 = InsertAddress("456 Oak Ave", "Boston", "MA", "02101");
        var personId1 = InsertPerson("John", "Doe");
        var personId2 = InsertPerson("Jane", "Smith");

        // Act
        InsertEmployee(addressId1, personId1, "TechCorp", "Developer", "John Doe");
        InsertEmployee(addressId2, personId2, "TechCorp", "Manager", "Jane Smith");

        // Assert
        var companyCount = _connection!.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Companies WHERE Name = @CompanyName",
            new { CompanyName = "TechCorp" });

        Assert.AreEqual(2, companyCount, "Should create separate companies for different addresses");
    }

    [TestMethod]
    public void EmployeeTrigger_MultipleEmployeesInserted_ShouldHandleCorrectly()
    {
        // Arrange
        var addressId1 = InsertAddress("123 Main St", "New York", "NY", "10001");
        var addressId2 = InsertAddress("456 Oak Ave", "Boston", "MA", "02101");
        var personId1 = InsertPerson("John", "Doe");
        var personId2 = InsertPerson("Jane", "Smith");

        // Act
        InsertEmployee(addressId1, personId1, "CompanyA", "Developer", "John Doe");
        InsertEmployee(addressId2, personId2, "CompanyB", "Manager", "Jane Smith");

        // Assert
        var companyAExists = _connection!.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Companies WHERE Name = @Name",
            new { Name = "CompanyA" });

        var companyBExists = _connection.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Companies WHERE Name = @Name",
            new { Name = "CompanyB" });

        Assert.AreEqual(1, companyAExists, "CompanyA should exist");
        Assert.AreEqual(1, companyBExists, "CompanyB should exist");
    }

    [TestMethod]
    public void EmployeeTrigger_WithNullOptionalFields_ShouldStillCreateCompany()
    {
        // Arrange
        var addressId = InsertAddress("123 Main St", null, null, null);
        var personId = InsertPerson("John", "Doe");

        // Act
        InsertEmployee(addressId, personId, "MinimalCorp", null, null);

        // Assert
        var companyExists = _connection!.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Companies WHERE Name = @Name",
            new { Name = "MinimalCorp" });

        Assert.AreEqual(1, companyExists, "Company should be created even with null optional fields");
    }

    private int InsertAddress(string street, string? city, string? state, string? zipCode)
    {
        _connection!.Execute(@"
            INSERT INTO Addresses (Street, City, State, ZipCode)
            VALUES (@Street, @City, @State, @ZipCode)",
            new { Street = street, City = city, State = state, ZipCode = zipCode });
        
        return (int)_connection.ExecuteScalar<long>("SELECT last_insert_rowid()");
    }

    private int InsertPerson(string firstName, string lastName)
    {
        _connection!.Execute(@"
            INSERT INTO Persons (FirstName, LastName)
            VALUES (@FirstName, @LastName)",
            new { FirstName = firstName, LastName = lastName });
        
        return (int)_connection.ExecuteScalar<long>("SELECT last_insert_rowid()");
    }

    private int InsertEmployee(int addressId, int personId, string companyName,
        string? position, string? employeeName)
    {
        _connection!.Execute(@"
            INSERT INTO Employees (AddressId, PersonId, CompanyName, Position, EmployeeName)
            VALUES (@AddressId, @PersonId, @CompanyName, @Position, @EmployeeName)",
            new
            {
                AddressId = addressId,
                PersonId = personId,
                CompanyName = companyName,
                Position = position,
                EmployeeName = employeeName
            });
        
        return (int)_connection.ExecuteScalar<long>("SELECT last_insert_rowid()");
    }

    private class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AddressId { get; set; }
    }
}