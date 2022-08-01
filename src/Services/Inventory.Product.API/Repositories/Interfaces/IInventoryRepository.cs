using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Abstractions;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(string itemNo, GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetAllByIdAsync(string id);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseItemDto model);
}