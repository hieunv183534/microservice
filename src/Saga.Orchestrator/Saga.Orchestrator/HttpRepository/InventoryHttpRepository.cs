using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    public Task<bool> CreateSalesOrder(string itemNo, SalesProductDto model)
    {
        throw new NotImplementedException();
    }
}