using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersPagingQuery : PagingRequestParameters, IRequest<PagedList<OrderDto>>
{
    public GetOrdersPagingQuery()
    {
        OrderBy = "CreatedDate Desc";
    }
}