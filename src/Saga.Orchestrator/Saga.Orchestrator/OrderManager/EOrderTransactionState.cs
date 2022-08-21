namespace Saga.Orchestrator.OrderManager;

public enum EOrderTransactionState
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    OrderCreated,
    OrderGot,
    OrderGetFailed,
    OrderCreatedFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    InventoryRollback,
    NotificationSent,
    NotificationSendFailed
}