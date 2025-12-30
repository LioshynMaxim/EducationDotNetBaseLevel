# Module 14 - Dapper Library Implementation Guide

## ?? Overview

This is a training template for implementing a data access layer using **Dapper** - a lightweight ORM for .NET.

### ? What's Already Done
- ? Models (Product, Order, OrderStatus)
- ? Repository interfaces (IRepository, IProductRepository, IOrderRepository)
- ? Data access interface (IDataAccess)
- ? Test project structure
- ? Test templates with TODO markers

### ?? Your Task
Implement the data access layer and repositories using Dapper.

---

## ?? Project Structure

```
DapperLibrary/
??? Models/
?   ??? Product.cs              ? Complete
?   ??? Order.cs                ? Complete
?   ??? OrderStatus.cs          ? Complete
??? Repositories/
?   ??? IRepository.cs          ? Complete
?   ??? IProductRepository.cs   ? Complete
?   ??? IOrderRepository.cs     ? Complete
?   ??? ProductRepository.cs    ? TODO: Implement
?   ??? OrderRepository.cs      ? TODO: Implement
??? DataAccess/
    ??? IDataAccess.cs          ? Complete
    ??? DataAccess.cs           ? TODO: Implement

DapperLibrary.Tests/
??? ProductRepositoryTests.cs   ? TODO: Implement tests
??? OrderRepositoryTests.cs     ? TODO: Implement tests
??? DataAccessTests.cs          ? TODO: Implement tests
```

---

## ?? Implementation Steps

### Step 1: Setup Database

1. Run `SQL/DatabaseSetup.sql` in SQL Server Management Studio
2. This creates:
   - Database `Module14DB`
   - `Products` table
   - `Orders` table
   - Sample data

### Step 2: Implement DataAccess.cs

**File:** `DapperLibrary/DataAccess/DataAccess.cs`

**TODO:**
```csharp
public IDbConnection GetConnection()
{
    return new SqlConnection(_connectionString);
}
```

**Test it:**
- Run `DataAccessTests.cs`
- All 4 tests should pass

---

### Step 3: Implement ProductRepository.cs

**File:** `DapperLibrary/Repositories/ProductRepository.cs`

#### 3.1 Create Method
```csharp
public void Create(Product entity)
{
    const string sql = @"INSERT INTO Products (Name, Description, Weight, Height, Width, Length)
                        VALUES (@Name, @Description, @Weight, @Height, @Width, @Length)";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, entity);
}
```

#### 3.2 Read Method
```csharp
public Product? Read(int id)
{
    const string sql = "SELECT * FROM Products WHERE Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    return connection.QueryFirstOrDefault<Product>(sql, new { Id = id });
}
```

#### 3.3 Update Method
```csharp
public void Update(Product entity)
{
    const string sql = @"UPDATE Products 
                        SET Name = @Name, Description = @Description, 
                            Weight = @Weight, Height = @Height, 
                            Width = @Width, Length = @Length
                        WHERE Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, entity);
}
```

#### 3.4 Delete Method
```csharp
public void Delete(int id)
{
    const string sql = "DELETE FROM Products WHERE Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, new { Id = id });
}
```

#### 3.5 GetAll Method
```csharp
public IEnumerable<Product> GetAll()
{
    const string sql = "SELECT * FROM Products";
    
    using var connection = _dataAccess.GetConnection();
    return connection.Query<Product>(sql);
}
```

**Test it:**
- Run `ProductRepositoryTests.cs`
- All tests should pass

---

### Step 4: Implement Stored Procedures

**File:** `SQL/DatabaseSetup.sql`

#### 4.1 sp_GetOrders
```sql
CREATE PROCEDURE sp_GetOrders
    @ProductId INT = NULL,
    @Status INT = NULL,
    @Year INT = NULL,
    @Month INT = NULL
AS
BEGIN
    SELECT o.*, p.*
    FROM Orders o
    LEFT JOIN Products p ON o.ProductId = p.Id
    WHERE (@ProductId IS NULL OR o.ProductId = @ProductId)
      AND (@Status IS NULL OR o.Status = @Status)
      AND (@Year IS NULL OR YEAR(o.CreateDate) = @Year)
      AND (@Month IS NULL OR MONTH(o.CreateDate) = @Month)
    ORDER BY o.CreateDate DESC
END
GO
```

#### 4.2 sp_DeleteOrdersBulk
```sql
CREATE PROCEDURE sp_DeleteOrdersBulk
    @ProductId INT = NULL,
    @Status INT = NULL,
    @Year INT = NULL,
    @Month INT = NULL
AS
BEGIN
    DELETE FROM Orders
    WHERE (@ProductId IS NULL OR ProductId = @ProductId)
      AND (@Status IS NULL OR Status = @Status)
      AND (@Year IS NULL OR YEAR(CreateDate) = @Year)
      AND (@Month IS NULL OR MONTH(CreateDate) = @Month)
    
    SELECT @@ROWCOUNT AS DeletedCount
END
GO
```

---

### Step 5: Implement OrderRepository.cs

**File:** `DapperLibrary/Repositories/OrderRepository.cs`

#### 5.1 Create Method
```csharp
public void Create(Order entity)
{
    const string sql = @"INSERT INTO Orders (Status, CreateDate, UpdateDate, ProductId)
                        VALUES (@Status, @CreateDate, @UpdateDate, @ProductId)";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, entity);
}
```

#### 5.2 Read Method (with Product mapping)
```csharp
public Order? Read(int id)
{
    const string sql = @"SELECT o.*, p.*
                        FROM Orders o
                        LEFT JOIN Products p ON o.ProductId = p.Id
                        WHERE o.Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    
    var orders = connection.Query<Order, Product, Order>(
        sql,
        (order, product) =>
        {
            order.Product = product;
            return order;
        },
        new { Id = id },
        splitOn: "Id"
    );
    
    return orders.FirstOrDefault();
}
```

#### 5.3 Update Method
```csharp
public void Update(Order entity)
{
    const string sql = @"UPDATE Orders 
                        SET Status = @Status, UpdateDate = @UpdateDate, ProductId = @ProductId
                        WHERE Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, entity);
}
```

#### 5.4 Delete Method
```csharp
public void Delete(int id)
{
    const string sql = "DELETE FROM Orders WHERE Id = @Id";
    
    using var connection = _dataAccess.GetConnection();
    connection.Execute(sql, new { Id = id });
}
```

#### 5.5 GetAll Method (with Product mapping)
```csharp
public IEnumerable<Order> GetAll()
{
    const string sql = @"SELECT o.*, p.*
                        FROM Orders o
                        LEFT JOIN Products p ON o.ProductId = p.Id";
    
    using var connection = _dataAccess.GetConnection();
    
    return connection.Query<Order, Product, Order>(
        sql,
        (order, product) =>
        {
            order.Product = product;
            return order;
        },
        splitOn: "Id"
    );
}
```

#### 5.6 GetOrders Method (using stored procedure)
```csharp
public IEnumerable<Order> GetOrders(int? productId = null, OrderStatus? status = null, 
                                    int? year = null, int? month = null)
{
    using var connection = _dataAccess.GetConnection();
    
    var parameters = new
    {
        ProductId = productId,
        Status = (int?)status,
        Year = year,
        Month = month
    };
    
    return connection.Query<Order, Product, Order>(
        "sp_GetOrders",
        (order, product) =>
        {
            order.Product = product;
            return order;
        },
        parameters,
        commandType: CommandType.StoredProcedure,
        splitOn: "Id"
    );
}
```

#### 5.7 DeleteOrdersBulk Method (using stored procedure)
```csharp
public int DeleteOrdersBulk(int? productId = null, OrderStatus? status = null, 
                            int? year = null, int? month = null)
{
    using var connection = _dataAccess.GetConnection();
    
    var parameters = new
    {
        ProductId = productId,
        Status = (int?)status,
        Year = year,
        Month = month
    };
    
    return connection.ExecuteScalar<int>(
        "sp_DeleteOrdersBulk",
        parameters,
        commandType: CommandType.StoredProcedure
    );
}
```

**Test it:**
- Run `OrderRepositoryTests.cs`
- All tests should pass

---

## ?? Testing Guide

### Running Tests

```bash
cd Module-14/DapperLibrary.Tests
dotnet test
```

### Test Categories

1. **DataAccessTests** - Connection management
2. **ProductRepositoryTests** - CRUD operations for products
3. **OrderRepositoryTests** - CRUD + custom queries for orders

### What to Test

? Create operations
? Read operations (single and all)
? Update operations
? Delete operations
? GetOrders with various filters
? DeleteOrdersBulk with various filters
? Null handling
? Empty result handling

---

## ?? Dapper Key Concepts

### 1. Execute (INSERT, UPDATE, DELETE)
```csharp
connection.Execute(sql, parameters);
```

### 2. Query (SELECT multiple)
```csharp
var items = connection.Query<Type>(sql, parameters);
```

### 3. QueryFirstOrDefault (SELECT single)
```csharp
var item = connection.QueryFirstOrDefault<Type>(sql, parameters);
```

### 4. ExecuteScalar (SELECT single value)
```csharp
var count = connection.ExecuteScalar<int>(sql, parameters);
```

### 5. Multi-Mapping (JOIN queries)
```csharp
connection.Query<Order, Product, Order>(
    sql,
    (order, product) =>
    {
        order.Product = product;
        return order;
    },
    splitOn: "Id"
);
```

### 6. Stored Procedures
```csharp
connection.Query<Type>(
    "sp_ProcedureName",
    parameters,
    commandType: CommandType.StoredProcedure
);
```

---

## ? Completion Checklist

### Implementation
- [ ] DataAccess.GetConnection()
- [ ] ProductRepository.Create()
- [ ] ProductRepository.Read()
- [ ] ProductRepository.Update()
- [ ] ProductRepository.Delete()
- [ ] ProductRepository.GetAll()
- [ ] sp_GetOrders stored procedure
- [ ] sp_DeleteOrdersBulk stored procedure
- [ ] OrderRepository.Create()
- [ ] OrderRepository.Read()
- [ ] OrderRepository.Update()
- [ ] OrderRepository.Delete()
- [ ] OrderRepository.GetAll()
- [ ] OrderRepository.GetOrders()
- [ ] OrderRepository.DeleteOrdersBulk()

### Testing
- [ ] All DataAccessTests pass
- [ ] All ProductRepositoryTests pass
- [ ] All OrderRepositoryTests pass
- [ ] Integration tests with real database work

---

## ?? Learning Objectives

After completing this exercise, you will understand:

? How to use Dapper for database operations
? Parameter binding and SQL injection prevention
? Multi-mapping for JOIN queries
? Stored procedure execution with Dapper
? Repository pattern implementation
? Unit testing with mocks
? Integration testing with real database

---

## ?? Common Issues & Solutions

### Issue 1: Connection String
**Problem:** Can't connect to database
**Solution:** Update connection string in your app:
```csharp
var connectionString = "Server=.;Database=Module14DB;Trusted_Connection=true;";
// OR
var connectionString = "Server=localhost;Database=Module14DB;Trusted_Connection=true;";
```

### Issue 2: Multi-Mapping
**Problem:** Product is null in Order
**Solution:** Make sure `splitOn` parameter matches the first column of second table:
```csharp
splitOn: "Id"  // First column of Product table
```

### Issue 3: Stored Procedure Not Found
**Problem:** "Could not find stored procedure"
**Solution:** 
1. Make sure you ran `DatabaseSetup.sql`
2. Check you're connected to correct database
3. Verify procedure exists: `SELECT * FROM sys.procedures`

---

## ?? Resources

- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [Dapper Tutorial](https://www.learndapper.com/)
- [SQL Server Stored Procedures](https://docs.microsoft.com/en-us/sql/relational-databases/stored-procedures/)

---

## ?? Next Steps

After completing Dapper implementation:
1. Move to EFLibrary for Entity Framework Core version
2. Compare performance and code complexity
3. Understand pros/cons of each approach

---

**Good luck with your implementation! ??**
