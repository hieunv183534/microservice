using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Order;

namespace Saga.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    public OrdersController(IOrderHttpRepository orderHttpRepository)
    {
        _orderHttpRepository = orderHttpRepository;
    }

    [HttpPost]
    public async Task<ActionResult<long>> CreateOrder([FromBody] CreateOrderDto model)
    {
        var result = await _orderHttpRepository.CreateOrder(model);
        return Ok(result);
    }
}