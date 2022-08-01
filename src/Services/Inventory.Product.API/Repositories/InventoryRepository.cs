using AutoMapper;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Repositories.Abstractions;
using Inventory.Product.API.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Repositories;

public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
{
    private readonly IMapper _mapper;
    public InventoryRepository(IMongoClient client, DatabaseSettings settings, IMapper mapper) : base(client, settings)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
    {
        var entities = await FindAll()
            .Find(x => x.ItemNo.Equals(itemNo))
            .ToListAsync();
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        
        return result;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(string itemNo, GetInventoryPagingQuery query)
    {
        FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Empty;
        if (!string.IsNullOrEmpty(query.SearchKeyword))
            filter = Builders<InventoryEntry>.Filter.Eq(s => s.DocumentNo, query.SearchKeyword);

        var totalRow = await Collection.Find(filter).CountDocumentsAsync();
        var entities = await Collection.Find(filter)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Limit(query.PageSize)
            .ToListAsync();
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        
        return result;
    }

    public async Task<InventoryEntryDto> GetAllByIdAsync(string id)
    {
        FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(s => s.Id, id);
        var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
        var result = _mapper.Map<InventoryEntryDto>(entity);

        return result;
    }

    public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseItemDto model)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = itemNo,
            Quantity = model.Quantity,
            DocumentType = model.DocumentType,
        };
        var entity = _mapper.Map<InventoryEntry>(itemToAdd);
        await CreateAsync(entity);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        
        return result;
    }
}