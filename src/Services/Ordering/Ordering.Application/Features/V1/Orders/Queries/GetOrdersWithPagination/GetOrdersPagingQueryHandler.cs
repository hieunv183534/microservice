using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Mappings;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersPagingQueryHandler : IRequestHandler<GetOrdersPagingQuery, PagedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    
    public GetOrdersPagingQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    
    public async Task<PagedList<OrderDto>> Handle(GetOrdersPagingQuery request, CancellationToken cancellationToken)
    {
        return await _orderRepository.FindAll()
            .OrderBy(x => x.CreatedDate)
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}