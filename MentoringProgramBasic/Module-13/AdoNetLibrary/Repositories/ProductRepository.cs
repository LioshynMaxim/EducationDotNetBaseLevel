using System.Data;
using System.Data.SqlClient;
using AdoNetLibrary.DataAccess;
using AdoNetLibrary.Models;

namespace AdoNetLibrary.Repositories;

public class ProductRepository(IDataAccess dataAccess) : IProductRepository
{
    private readonly IDataAccess _dataAccess = dataAccess;

    public void Create(Product product)
    {
        const string sql = @"INSERT INTO Products (Name, Description, Weight, Height, Width, Length)
                            VALUES (@Name, @Description, @Weight, @Height, @Width, @Length)";

        var parameters = new[]
        {
            new SqlParameter("@Name", product.Name ?? string.Empty),
            new SqlParameter("@Description", product.Description ?? string.Empty),
            new SqlParameter("@Weight", product.Weight),
            new SqlParameter("@Height", product.Height),
            new SqlParameter("@Width", product.Width),
            new SqlParameter("@Length", product.Length)
        };

        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public Product? Read(int id)
    {
        const string sql = @"SELECT Id, Name, Description, Weight, Height, Width, Length 
                            FROM Products WHERE Id = @Id";

        var parameters = new[] { new SqlParameter("@Id", id) };
        var dataTable = _dataAccess.ExecuteQuery(sql, CommandType.Text, parameters);

        var product = dataTable.AsEnumerable()
            .Where(row => Convert.ToInt32(row["Id"]) == id)
            .Select(MapToProduct)
            .FirstOrDefault();

        return product;
    }

    public void Update(Product product)
    {
        const string sql = @"UPDATE Products 
                            SET Name = @Name, Description = @Description, 
                                Weight = @Weight, Height = @Height, 
                                Width = @Width, Length = @Length
                            WHERE Id = @Id";

        var parameters = new[]
        {
            new SqlParameter("@Id", product.Id),
            new SqlParameter("@Name", product.Name ?? string.Empty),
            new SqlParameter("@Description", product.Description ?? string.Empty),
            new SqlParameter("@Weight", product.Weight),
            new SqlParameter("@Height", product.Height),
            new SqlParameter("@Width", product.Width),
            new SqlParameter("@Length", product.Length)
        };

        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";
        var parameters = new[] { new SqlParameter("@Id", id) };
        _dataAccess.ExecuteNonQuery(sql, CommandType.Text, parameters);
    }

    public IEnumerable<Product> GetAll()
    {
        const string sql = "SELECT Id, Name, Description, Weight, Height, Width, Length FROM Products";
        var dataTable = _dataAccess.ExecuteQuery(sql);

        return dataTable == null ? [] : [.. dataTable.AsEnumerable().Select(MapToProduct)];
    }

    private static Product MapToProduct(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["Id"]),
        Name = row["Name"].ToString() ?? string.Empty,
        Description = row["Description"].ToString() ?? string.Empty,
        Weight = Convert.ToDecimal(row["Weight"]),
        Height = Convert.ToDecimal(row["Height"]),
        Width = Convert.ToDecimal(row["Width"]),
        Length = Convert.ToDecimal(row["Length"])
    };
}
