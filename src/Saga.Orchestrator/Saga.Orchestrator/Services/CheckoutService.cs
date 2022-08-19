using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    public CheckoutSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository, IMapper mapper, IInventoryHttpRepository inventoryHttpRepository)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _mapper = mapper;
        _inventoryHttpRepository = inventoryHttpRepository;
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
        var addedOrder = await _orderHttpRepository.GetOrder(orderId);
        
        // Create Sales Order from InventoryRepository
        foreach (var item in cart.Items)
        {
            var saleOrder = new SalesProductDto(addedOrder.DocumentNo, item.Quantity);
            saleOrder.SetItemNo(item.ItemNo);
            await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
        }

        return true;
    }
}