using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    private readonly HttpClient _client;

    public InventoryHttpRepository(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<string> CreateSalesOrder(SalesProductDto model)
    {
        var response = await _client.PostAsJsonAsync($"inventory/sales/{model.ItemNo}", model);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Create sale order for item: {model.ItemNo} not success");

        var inventory = await response.ReadContentAs<InventoryEntryDto>();
        return inventory.DocumentNo;
    }

    public async Task<string> CreateOrderSale(string orderNo, SalesOrderDto model)
    {
        var response = await _client.PostAsJsonAsync($"inventory/sales/order-no/{orderNo}",
            model);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Create sale order for Order No: {orderNo} not success");

        var result = await response.ReadContentAs<CreatedSalesOrderSuccessDto>();
        return result.DocumentNo;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _client.DeleteAsync($"inventory/document-no/{documentNo}");
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete order for Document No: {documentNo} not success");

        var result = response.IsSuccessStatusCode;
        return result;
    }
}