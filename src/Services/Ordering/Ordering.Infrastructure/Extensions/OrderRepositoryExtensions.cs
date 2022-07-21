using Infrastructure.Extensions.Utility;
using Ordering.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace Ordering.Infrastructure.Extensions;

public static class OrderRepositoryExtensions
{
    public static IQueryable<Order> Sort(this IQueryable<Order> parameter, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return parameter.OrderBy(e => e.CreatedDate);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Order>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return parameter.OrderBy(e => e.CreatedDate);

        return parameter.OrderBy(orderQuery);
    }
}