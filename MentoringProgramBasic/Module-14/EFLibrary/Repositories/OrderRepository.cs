using System.Linq.Expressions;
using EFLibrary.DataAccess;
using InfrastructureLibraryAdoEfDapper.Interfaces;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EFLibrary.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;

    public OrderRepository(ApplicationDbContext context, IProductRepository productRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public void Create(Order entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var product = _productRepository.Read(entity.ProductId);
        if (product == null)
            throw new ArgumentException($"Product with ID {entity.ProductId} not found.", nameof(entity.ProductId));

        _context.Orders.Add(entity);
        _context.SaveChanges();
    }

    public Order? Read(int id) => _context.Orders
        .Include(o => o.Product)
        .FirstOrDefault(o => o.Id == id);

    public void Update(Order entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var product = _productRepository.Read(entity.ProductId);
        if (product == null)
            throw new ArgumentException($"Product with ID {entity.ProductId} not found.", nameof(entity.ProductId));

        var order = Read(entity.Id) ?? throw new ArgumentException($"Order with ID {entity.Id} not found.");
        _context.Entry(order).CurrentValues.SetValues(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var order = Read(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Order> GetAll() => _context.Orders
        .AsNoTracking()
        .Include(o => o.Product)
        .ToList();

    public IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> predicate)
    {
        return _context.Orders
            .AsNoTracking()
            .Include(o => o.Product)
            .Where(predicate)
            .ToList();
    }

    public int DeleteOrdersBulk(Expression<Func<Order, bool>> predicate)
    {
        var ordersToDelete = _context.Orders.Where(predicate).ToList();
        _context.Orders.RemoveRange(ordersToDelete);
        _context.SaveChanges();
        return ordersToDelete.Count;
    }
}
