using Infrastructure.Common;
using Inventory.Product.Grpc.Entities;
using Inventory.Product.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Product.Grpc.Repositories;

public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
{
    public InventoryRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
    {
    }

    public Task<int> GetStockQuantity(string itemNo)
    {
        throw new NotImplementedException();
    }
}