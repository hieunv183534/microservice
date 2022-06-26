using Contracts.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderOrderRepository : OrderRepositoryBase<Order, long>, IOrderRepository
{
    public OrderOrderRepository(OrderContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
    {
        var result = FindByCondition(x => x.UserName.Equals(userName));
        return await result.ToListAsync();
    }
}