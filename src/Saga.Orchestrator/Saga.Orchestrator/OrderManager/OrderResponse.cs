namespace Saga.Orchestrator.OrderManager;

public class OrderResponse
{
    public OrderResponse(bool success)
    {
        Success = success;
    }
    public bool Success { get; private set; }
}