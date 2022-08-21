using System.ComponentModel.DataAnnotations;
using Contracts.Sagas.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.OrderManager;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _orderManager;

    public CheckoutController(ISagaOrderManager<BasketCheckoutDto, OrderResponse> orderManager)
    {
        _orderManager = orderManager;
    }

    [HttpPost]
    [Route("{username}")]
    public OrderResponse CheckoutOrder([Required] string username,
        [FromBody] BasketCheckoutDto model)
    {
        model.UserName = username;
        var result = _orderManager.CreateOrder(model);

        return result;
    }
}