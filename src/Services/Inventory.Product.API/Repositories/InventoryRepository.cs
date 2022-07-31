using System.Linq.Expressions;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Persistence.Interfaces;
using Inventory.Product.API.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Inventory.Product.API.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IInventoryContext _dbContext;
    
    public InventoryRepository(IInventoryContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); 
    }
    
    public async Task<IEnumerable<InventoryEntry>> GetInventories(string itemNo)
    {
        FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Empty;
        if (!string.IsNullOrEmpty(itemNo))
            filter = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, itemNo);

        return await _dbContext.InventoryEntries
            .Find(filter).ToListAsync();
    }
    
    public IQueryable<InventoryEntry> FindAll(bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<InventoryEntry> FindAll(bool trackChanges = false, params Expression<Func<InventoryEntry, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public IQueryable<InventoryEntry> FindByCondition(Expression<Func<InventoryEntry, bool>> expression, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public IQueryable<InventoryEntry> FindByCondition(Expression<Func<InventoryEntry, bool>> expression, bool trackChanges = false, params Expression<Func<InventoryEntry, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<InventoryEntry?> GetByIdAsync(ObjectId id)
    {
        throw new NotImplementedException();
    }

    public Task<InventoryEntry?> GetByIdAsync(ObjectId id, params Expression<Func<InventoryEntry, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }
}