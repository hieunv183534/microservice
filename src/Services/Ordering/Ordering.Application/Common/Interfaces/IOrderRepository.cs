using Contracts.Common.Interfaces;
using Ordering.Application.Features.V1.Orders;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<PagedList<Order>> GetOrderPagination(GetOrderParameters parameters);
    Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName);
    void CreateOrder(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    void DeleteOrder(Order order);
}