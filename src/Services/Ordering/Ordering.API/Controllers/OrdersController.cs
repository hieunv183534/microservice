using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Shared.DTOs.Order;
using Shared.SeedWork;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public OrdersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
    }

    #region CRUD

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    {
        var query = new GetOrdersByUserNameQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost(Name = RouteNames.CreateOrder)]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody]CreateOrderDto model)
    {
        var command = _mapper.Map<CreateOrderCommand>(model);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPut("{id:long}",Name = RouteNames.UpdateOrder)]
    [ProducesResponseType(typeof(ApiResult<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDto>> UpdateOrder([Required]long id, [FromBody]UpdateOrderCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpDelete("{id:long}",Name = RouteNames.DeleteOrder)]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteOrder([Required]long id)
    {
        var command = new DeleteOrderCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
    
    #endregion
}