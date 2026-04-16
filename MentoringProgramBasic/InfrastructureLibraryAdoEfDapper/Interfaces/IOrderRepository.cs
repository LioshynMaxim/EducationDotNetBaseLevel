using System.Linq.Expressions;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;

namespace InfrastructureLibraryAdoEfDapper.Interfaces;

public interface IOrderRepository: IRepository<Order>
{
    IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> predicate);
    int DeleteOrdersBulk(Expression<Func<Order, bool>> predicate);
}
