using AdoNetLibrary.Models;

namespace AdoNetLibrary.Repositories;

public interface IOrderRepository: IRepository<Order>
{
    IEnumerable<Order> GetOrders(int? productId = null, OrderStatus? status = null, int? year = null, int? month = null);
    int DeleteOrdersBulk(int? productId = null, OrderStatus? status = null, int? year = null, int? month = null);
}
