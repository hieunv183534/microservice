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
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public SagaOrderManager(IOrderHttpRepository orderHttpRepository, IInventoryHttpRepository inventoryHttpRepository,
        IBasketHttpRepository basketHttpRepository, IMapper mapper, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _basketHttpRepository = basketHttpRepository;
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
        string? inventoryDocumentNo;

        orderStateMachine.Configure(EOrderTransactionState.NotStarted)
            .PermitDynamic(EOrderAction.GetBasket, () =>
            {
                cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGetFailed;
            });

        orderStateMachine.Configure(EOrderTransactionState.BasketGot)
            .PermitDynamic(EOrderAction.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreatedFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(EOrderAction.CreateOrder));
            

        orderStateMachine.Configure(EOrderTransactionState.OrderCreated)
            .PermitDynamic(EOrderAction.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrderGetFailed;
            })
            .OnEntry(() => orderStateMachine.Fire(EOrderAction.GetOrder));

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
            })
            .OnEntry(() => orderStateMachine.Fire(EOrderAction.UpdateInventory));

        orderStateMachine.Fire(EOrderAction.GetBasket);

        return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryUpdated);
    }

    public OrderResponse RollbackOrder(BasketCheckoutDto input)
    {
        return new OrderResponse(false);
    }
}