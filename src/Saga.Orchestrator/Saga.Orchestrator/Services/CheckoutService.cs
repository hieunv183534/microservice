using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Order;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IMapper _mapper;
    public CheckoutSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository, IMapper mapper)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _mapper = mapper;
    }
    
    public async Task<bool> Checkout(string username, BasketCheckoutDto basketCheckout)
    {
        // Get cart from BasketHttpRepository
        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null) return false;
        
        // Create Order from OrderHttpRepository
        var order = _mapper.Map<CreateOrderDto>(basketCheckout);
        order.TotalPrice = cart.TotalPrice;
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId < 0) return false;

        return true;
    }
}