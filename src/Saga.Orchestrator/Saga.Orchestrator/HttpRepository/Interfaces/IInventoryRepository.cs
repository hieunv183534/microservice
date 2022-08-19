using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IInventoryHttpRepository
{
    Task<bool> CreateSalesOrder(string itemNo, SalesProductDto model);
}