using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<ApiResult<OrderDto>>
{
    public long Id { get; set; }

    public GetOrderByIdQuery(long id)
    {
        Id = id;
    }
}