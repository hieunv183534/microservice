using Contracts.Domains.Interfaces;
using Inventory.Product.API.Entities;
using MongoDB.Bson;

namespace Inventory.Product.API.Repositories.Interfaces;

public interface IInventoryRepository : IRepositoryQueryBase<InventoryEntry, ObjectId>
{
    Task<IEnumerable<InventoryEntry>> GetInventories(string itemNo);
}