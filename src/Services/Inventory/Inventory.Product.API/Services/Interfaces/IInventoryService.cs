using Contracts.Domains.Interfaces;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Services.Interfaces;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetByIdAsync(string id);
    Task DeleteByDocumentNoAsync(string documentNo);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model);
    Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model);
    Task<string> SalesOrderAsync(SalesOrderDto model);
}