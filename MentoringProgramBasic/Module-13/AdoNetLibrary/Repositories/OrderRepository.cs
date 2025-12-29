using System.Data;
using System.Data.SqlClient;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;

namespace AdoNetLibrary.Repositories;

public class OrderRepository(IDataAccess dataAccess) : IOrderRepository
{
    private readonly IDataAccess _dataAccess = dataAccess;

    public void Create(Order order)
    {
        const string sql = @"INSERT INTO Orders (Status, CreateDate, UpdateDate, ProductId)
                            VALUES (@Status, @CreateDate, @UpdateDate, @ProductId)";

        var parameters = new[]
        {
            new SqlParameter("@Status", (int)order.Status),
            new SqlParameter("@CreateDate", order.CreateDate),
            new SqlParameter("@UpdateDate", order.UpdateDate),
            new SqlParameter("@ProductId", order.ProductId)
        };

        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public Order? Read(int id)
    {
        const string sql = @"SELECT Id, Status, CreateDate, UpdateDate, ProductId 
                            FROM Orders WHERE Id = @Id";

        var parameters = new[] { new SqlParameter("@Id", id) };
        var dataTable = _dataAccess.ExecuteQuery(sql, CommandType.Text, parameters);

        var order = dataTable.AsEnumerable()
            .Where(row => Convert.ToInt32(row["Id"]) == id)
            .Select(MapToOrder)
            .FirstOrDefault();

        return order;
    }

    public void Update(Order order)
    {
        const string sql = @"UPDATE Orders 
                            SET Status = @Status, UpdateDate = @UpdateDate, ProductId = @ProductId
                            WHERE Id = @Id";

        var parameters = new[]
        {
            new SqlParameter("@Id", order.Id),
            new SqlParameter("@Status", (int)order.Status),
            new SqlParameter("@UpdateDate", order.UpdateDate),
            new SqlParameter("@ProductId", order.ProductId)
        };

        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Orders WHERE Id = @Id";
        var parameters = new[] { new SqlParameter("@Id", id) };
        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public IEnumerable<Order> GetOrders(int? productId = null, OrderStatus? status = null, int? year = null, int? month = null)
    {
        const string sql = "SELECT Id, Status, CreateDate, UpdateDate, ProductId FROM Orders";
        var dataTable = _dataAccess.ExecuteQuery(sql);

        var query = dataTable.AsEnumerable().Select(MapToOrder);

        if (productId.HasValue)
        {
            query = query.Where(o => o.ProductId == productId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(o => o.CreateDate.Year == year.Value);
        }
        if (month.HasValue)
        {
            query = query.Where(o => o.CreateDate.Month == month.Value);
        }

        return query.OrderByDescending(o => o.CreateDate).ToList();
    }

    public int DeleteOrdersBulk(int? productId = null, OrderStatus? status = null, int? year = null, int? month = null)
    {
        var ordersToDelete = GetOrders(productId, status, year, month).ToList();

        int deletedCount = 0;
        foreach (var order in ordersToDelete)
        {
            Delete(order.Id);
            deletedCount++;
        }

        return deletedCount;
    }

    public IEnumerable<Order> GetAll() 
    {
        const string sql = "SELECT Id, Status, CreateDate, UpdateDate, ProductId FROM Orders";
        var dataTable = _dataAccess.ExecuteQuery(sql);
        return dataTable.AsEnumerable().Select(MapToOrder);
    }

    private static Order MapToOrder(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["Id"]),
        Status = (OrderStatus)Convert.ToInt32(row["Status"]),
        CreateDate = Convert.ToDateTime(row["CreateDate"]),
        UpdateDate = Convert.ToDateTime(row["UpdateDate"]),
        ProductId = Convert.ToInt32(row["ProductId"])
    };
}
