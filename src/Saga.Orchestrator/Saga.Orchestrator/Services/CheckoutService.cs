using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    public CheckoutSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
    }
    
    public async Task<bool> Checkout(string username, BasketCheckoutDto basketCheckout)
    {
        // Get cart from BasketHttpRepository
        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null) return false;

        return true;
    }
}