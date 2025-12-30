using DapperLibrary.DataAccess;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;

namespace DapperLibrary.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDataAccess _dataAccess;

    public ProductRepository(IDataAccess dataAccess)
    {
        ArgumentNullException.ThrowIfNull(dataAccess);
        _dataAccess = dataAccess;
    }

    public void Create(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dataAccess.Execute(
            "INSERT INTO Products (Name, Description, Weight, Height, Width, Length) " +
            "VALUES (@Name, @Description, @Weight, @Height, @Width, @Length)",
            new
            {
                entity.Name,
                entity.Description,
                entity.Weight,
                entity.Height,
                entity.Width,
                entity.Length
            });
    }

    public Product? Read(int id)
    {
        return _dataAccess.Execute<Product>(
            "SELECT Id, Name, Description, Weight, Height, Width, Length " +
            "FROM Products WHERE Id = @Id",
            new { Id = id });
    }

    public void Update(Product entity)
    {
        _dataAccess.Execute(
            "UPDATE Products SET Name = @Name, Description = @Description, Weight = @Weight, Height = @Height, Width = @Width, Length = @Length " +
            "WHERE Id = @Id",
            new
            {
                entity.Name,
                entity.Description,
                entity.Weight,
                entity.Height,
                entity.Width,
                entity.Length,
                entity.Id
            });
    }

    public void Delete(int id)
    {
        _dataAccess.Execute(
            "DELETE FROM Products WHERE Id = @Id",
            new { Id = id });
    }

    public IEnumerable<Product> GetAll()
    {
        return _dataAccess.Query<Product>(
            "SELECT Id, Name, Description, Weight, Height, Width, Length FROM Products",
            null);
    }
}
