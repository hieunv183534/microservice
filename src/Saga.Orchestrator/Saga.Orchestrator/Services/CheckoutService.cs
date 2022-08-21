using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    public CheckoutSagaService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository, IMapper mapper, IInventoryHttpRepository inventoryHttpRepository, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _mapper = mapper;
        _inventoryHttpRepository = inventoryHttpRepository;
        _logger = logger;
    }
    
    public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout)
    {
        // Get cart from BasketHttpRepository
        _logger.Information($"Start: Get Cart {username}");

        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null) return false;
        _logger.Information($"End: Get Cart {username} success");
        
        // Create Order from OrderHttpRepository
        _logger.Information($"Starting: Create Order");

        var order = _mapper.Map<CreateOrderDto>(basketCheckout);
        order.TotalPrice = cart.TotalPrice;
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId < 0) return false;
        var addedOrder = await _orderHttpRepository.GetOrder(orderId);
        _logger.Information($"End: Created Order success, Order Id: {orderId} - DocumentNo: {addedOrder.DocumentNo}");
        
        var inventoryDocumentNos = new List<string>();
        bool result;
        try
        {
            // Sales Items from InventoryRepository
            foreach (var item in cart.Items)
            {
                _logger.Information($"Start: Sale Item No: {item.ItemNo}, Quantity: {item.Quantity}");

                var saleOrder = new SalesProductDto(addedOrder.DocumentNo, item.Quantity);
                saleOrder.SetItemNo(item.ItemNo);
                var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                inventoryDocumentNos.Add(documentNo);
                _logger.Information($"End: Sale Item No: {item.ItemNo}, Quantity: {item.Quantity}, DocumentNo: {documentNo}");
            }
            
            result = await _basketHttpRepository.DeleteBasket(username);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            RollbackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNos);
            result = false;
        }

        return result;
    }

    private async Task RollbackCheckoutOrder(string username, long orderId, List<string> inventoryDocumentNos)
    {
        _logger.Information($"Start: RollbackCheckoutOrder for username: {username}, " +
                            $"order id: {orderId}, " +
                            $"inventory document nos: {String.Join(", ", inventoryDocumentNos)}");

        // delete order by Order's DocumentNo
        _logger.Information($"Start: Delete Order Id: {orderId}");
        await _orderHttpRepository.DeleteOrder(orderId);
        _logger.Information($"End: Deleted Order Id: {orderId}");

        var deletedDocumentNos = new List<string>();
        _logger.Information($"Start: Delete Inventory Document No");
        foreach (var documentNo in inventoryDocumentNos)
        {
            await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
            deletedDocumentNos.Add(documentNo);
        }
        _logger.Information($"End: Deleted Inventory Document Nos : {String.Join(", ", deletedDocumentNos)}");
    }
}