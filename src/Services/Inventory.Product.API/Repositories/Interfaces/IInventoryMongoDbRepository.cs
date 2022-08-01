using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Abstractions;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryMongoDbRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntry>> GetAllByItemNoAsync(string itemNo);
    Task<IEnumerable<InventoryEntry>> GetAllByItemNoPagingAsync(string itemNo, GetInventoryPagingQuery query);
    Task<InventoryEntry> GetAllByIdAsync(string id);
}