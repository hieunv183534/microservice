using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Contracts.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Ordering.Domain.Entities;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageProducer _messageProducer;
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public OrdersController(IMediator mediator, IMessageProducer messageProducer, IOrderRepository repository, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _repository = repository;
        _mapper = mapper;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    {
        var query = new GetOrdersQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderDto orderDto)
    {
        var order = _mapper.Map<Order>(orderDto);
        var addedOrder = await _repository.CreateOrder(order);
        await _repository.SaveChangesAsync();
        var result = _mapper.Map<OrderDto>(addedOrder);
        _messageProducer.SendMessage(result);
        return Ok(result);
    }
}