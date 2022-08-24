using AutoMapper;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.OrderManager;

public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public SagaOrderManager(IOrderHttpRepository orderHttpRepository, 
        IBasketHttpRepository basketHttpRepository, 
        IInventoryHttpRepository inventoryHttpRepository, 
        IMapper mapper, 
        ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public OrderResponse CreateOrder(BasketCheckoutDto input)
    {
        var orderStateMachine =
            new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.NotStarted);

        long orderId = -1;
        CartDto cart = null;
        OrderDto addedOrder = null;
        string? inventoryDocumentNo = null;

        //1. Get Basket by Username
        orderStateMachine.Configure(EOrderTransactionState.NotStarted)
            .PermitDynamic(EOrderAction.GetBasket, () =>
            {
                cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGetFailed;
            });

        //2. Create order (Place Order)
        orderStateMachine.Configure(EOrderTransactionState.BasketGot)
            .PermitDynamic(EOrderAction.CreateOrder, () =>
            {
                // input.TotalPrice = cart.TotalPrice; //use this is fine.
                var order = _mapper.Map<CreateOrderDto>(input);
                order.TotalPrice = cart.TotalPrice;
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreatedFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(EOrderAction.CreateOrder));

        //3. Get Order detail by Order Id
        orderStateMachine.Configure(EOrderTransactionState.OrderCreated)
            .PermitDynamic(EOrderAction.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrderGetFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(EOrderAction.GetOrder));

        //4. Inventory update
        orderStateMachine.Configure(EOrderTransactionState.OrderGot)
            .PermitDynamic(EOrderAction.UpdateInventory, () =>
            {
                var salesOrder = new SalesOrderDto()
                {
                    OrderNo = addedOrder.DocumentNo,
                    SaleItems = _mapper.Map<List<SaleItemDto>>(cart.Items)
                };
                inventoryDocumentNo =
                    _inventoryHttpRepository.CreateOrderSale(addedOrder.DocumentNo, salesOrder).Result;
                return inventoryDocumentNo != null
                    ? EOrderTransactionState.InventoryUpdated
                    : EOrderTransactionState.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.UpdateInventory));

        //5. Delete Basket
        orderStateMachine.Configure(EOrderTransactionState.InventoryUpdated)
            .PermitDynamic(EOrderAction.DeleteBasket, () =>
            {
                var result = _basketHttpRepository.DeleteBasket(input.UserName).Result;
                return result ? EOrderTransactionState.BasketDeleted : EOrderTransactionState.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteBasket));

        // Rollback order
        orderStateMachine.Configure(EOrderTransactionState.InventoryUpdateFailed)
            .PermitDynamic(EOrderAction.DeleteInventory, () =>
            {
                RollbackOrder(input.UserName, inventoryDocumentNo, orderId);
                return EOrderTransactionState.InventoryRollback;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteInventory));

        orderStateMachine.Fire(EOrderAction.GetBasket);
        
        return new OrderResponse(orderStateMachine.State == EOrderTransactionState.BasketDeleted);
    }

    public OrderResponse RollbackOrder(string username, string documentNo, long orderId)
    {
        var orderStateMachine =
            new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.RollbackInventory);

        //1. Delete Inventory by Document No
        orderStateMachine.Configure(EOrderTransactionState.RollbackInventory)
            .PermitDynamic(EOrderAction.DeleteInventory, () =>
            {
                _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
                return EOrderTransactionState.InventoryRollback;
            });
        
        //2. Delete Order by order id
        orderStateMachine.Configure(EOrderTransactionState.InventoryRollback)
            .PermitDynamic(EOrderAction.DeleteOrder, () =>
            {
                var result = _orderHttpRepository.DeleteOrder(orderId).Result;
                return result ? EOrderTransactionState.OrderDeleted : EOrderTransactionState.OrderDeletedFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteOrder));
        
        orderStateMachine.Fire(EOrderAction.DeleteInventory);

        return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryRollback);
    }
}