using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderPagingQuery : GetOrderParameters, IRequest<PagedList<OrderDto>>
{
   
}