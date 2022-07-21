using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersWithPaginationQuery : PagingRequestParameters, IRequest<PagedList<OrderDto>>
{
    public GetOrdersWithPaginationQuery()
    {
        OrderBy = "CreatedDate Desc";
    }
}