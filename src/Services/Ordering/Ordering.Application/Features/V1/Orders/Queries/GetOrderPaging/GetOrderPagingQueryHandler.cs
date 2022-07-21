using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Mappings;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderPagingQueryHandler : IRequestHandler<GetOrderPagingQuery, PagedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    
    public GetOrderPagingQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    
    public async Task<PagedList<OrderDto>> Handle(GetOrderPagingQuery request, CancellationToken cancellationToken)
    {
        var pagedList = await _orderRepository.GetOrderPagination(request);
        var items = _mapper.Map<List<OrderDto>>(pagedList);

        var result = new PagedList<OrderDto>(items, pagedList.GetMetaData().TotalItems, request.PageNumber,
            request.PageSize);

        return result;
    }
}