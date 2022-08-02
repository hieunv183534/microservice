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

    public async Task<int> GetStockQuantity(string itemNo) 
        => Collection.AsQueryable()
            .Where(x => x.ItemNo.Equals(itemNo))
            .Sum(x => x.Quantity);

}