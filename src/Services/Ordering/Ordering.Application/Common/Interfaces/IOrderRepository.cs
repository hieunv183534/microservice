using Contracts.Domains.Interfaces;
using Ordering.Application.Features.V1.Orders;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBase<Order, long>
{
    IQueryable<Order> GetOrderPaginationQueryable(GetOrderParameters parameters);
    Task<PagedList<Order>> GetOrderPagination(GetOrderParameters parameters);
    Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName);
    void CreateOrder(Order order);
    Task<Order> UpdateOrderAsync(Order order);
    void DeleteOrder(Order order);
}