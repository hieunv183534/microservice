using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Saga.Orchestrator.HttpRepository;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    private readonly HttpClient _client;
    public InventoryHttpRepository(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<bool> CreateSalesOrder(SalesProductDto model)
    {
        var response = await _client.PostAsJsonAsync($"inventory/sales/{model.ItemNo}", model);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode) return false;

        var orderId = await response.ReadContentAs<InventoryEntryDto>();
        return orderId.Id != null;
    }
}