using Contracts.Domains.Interfaces;
using Inventory.Product.Grpc.Entities;

namespace Inventory.Product.Grpc.Repositories.Interfaces;

public interface IInventoryRepository : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<int> GetStockQuantity(string itemNo);
}