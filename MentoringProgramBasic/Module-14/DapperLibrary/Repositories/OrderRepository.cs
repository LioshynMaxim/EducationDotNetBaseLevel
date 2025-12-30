using System.Linq.Expressions;
using DapperLibrary.DataAccess;
using InfrastructureLibraryAdoEfDapper.Interfaces;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;

namespace DapperLibrary.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDataAccess _dataAccess;
    private readonly IProductRepository _productRepository;

    public OrderRepository(IDataAccess dataAccess, IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(dataAccess);
        ArgumentNullException.ThrowIfNull(productRepository);
        
        _dataAccess = dataAccess;
        _productRepository = productRepository;
    }

    public void Create(Order entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var product = _productRepository.Read(entity.ProductId);
        if (product == null)
            throw new ArgumentException($"Product with ID {entity.ProductId} not found.", nameof(entity.ProductId));

        _dataAccess.Execute(
            "INSERT INTO Orders (Status, CreateDate, UpdateDate, ProductId) " +
            "VALUES (@Status, @CreateDate, @UpdateDate, @ProductId)",
            new
            {
                entity.Status,
                entity.CreateDate,
                entity.UpdateDate,
                entity.ProductId
            });
    }

    public Order? Read(int id)
    {
        return _dataAccess.Execute<Order>(
            "SELECT Id, Status, CreateDate, UpdateDate, ProductId FROM Orders WHERE Id = @Id",
            new { Id = id });
    }

    public void Update(Order entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var product = _productRepository.Read(entity.ProductId);
        if (product == null)
            throw new ArgumentException($"Product with ID {entity.ProductId} not found.", nameof(entity.ProductId));

        _dataAccess.Execute(
            "UPDATE Orders SET Status = @Status, CreateDate = @CreateDate, " +
            "UpdateDate = @UpdateDate, ProductId = @ProductId WHERE Id = @Id",
            new
            {
                entity.Status,
                entity.CreateDate,
                entity.UpdateDate,
                entity.ProductId,
                entity.Id
            });
    }

    public void Delete(int id)
    {
        _dataAccess.Execute(
            "DELETE FROM Orders WHERE Id = @Id",
            new { Id = id });
    }

    public IEnumerable<Order> GetAll()
    {
        return _dataAccess.Query<Order>(
            "SELECT Id, Status, CreateDate, UpdateDate, ProductId FROM Orders",
            null);
    }

    public IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var allOrders = GetAll();
        return allOrders.Where(compiledPredicate);
    }

    public int DeleteOrdersBulk(Expression<Func<Order, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var ordersToDelete = GetAll().Where(compiledPredicate).ToList();

        _dataAccess.Execute(
            "DELETE FROM Orders WHERE Id IN @Ids",
            new { Ids = ordersToDelete.Select(o => o.Id).ToArray() });
        
        return ordersToDelete.Count;
    }
}
