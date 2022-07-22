using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Extensions;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderPagingQueryHandler : IRequestHandler<GetOrderPagingQuery, ApiResult<PagedList<OrderDto>>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public GetOrderPagingQueryHandler(IOrderRepository repository, IMapper mapper, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(repository));
    }
    
    private const string MethodName = "GetOrderPagingQueryHandler";

    public async Task<ApiResult<PagedList<OrderDto>>> Handle(GetOrderPagingQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - pageNumber: {request.PageNumber}, pageSize: {request.PageSize}, orderby: {request.OrderBy}, searchTerm: {request.SearchTerm}");

        //1. using ProjectTo
        var pagedList = _repository.GetOrderPaginationQueryable(request);
        var result = await pagedList.ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        _logger.Information($"END: {MethodName} - pageNumber: {request.PageNumber}, pageSize: {request.PageSize}, orderby: {request.OrderBy}, searchTerm: {request.SearchTerm}");
        return new ApiSuccessResult<PagedList<OrderDto>>(result);
        
        //2. using Map
        // var pagedList = await _orderRepository.GetOrderPagination(request);
        // var items = _mapper.Map<List<OrderDto>>(pagedList);
        //
        // var result = new PagedList<OrderDto>(items, pagedList.GetMetaData().TotalItems, request.PageNumber,
        //     request.PageSize);
        //
        // return result;
    }
}