using Shared.DTOs.Order;

namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IOrderHttpRepository
{
    Task<long> CreateOrder(CreateOrderDto order);
    Task<bool> DeleteOrder(long id);
}