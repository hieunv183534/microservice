using System.ComponentModel.DataAnnotations;
using System.Net;
using Inventory.Product.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _repository;

    public InventoryController(IInventoryRepository repository)
    {
        _repository = repository;
    }
    
    [Route("items/{itemNo}", Name = "GetAllByItemNo")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required]string itemNo)
    {
        var result = await _repository.GetAllByItemNoAsync(itemNo);
        return Ok(result);
    }
    
    [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPagingAsync")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNoPagingAsync([Required]string itemNo, [FromQuery] GetInventoryPagingQuery query)
    {
        var result = await _repository
            .GetAllByItemNoPagingAsync(itemNo, query);
        return Ok(result);
    }
    
    [Route("{id}", Name = "GetInventoryById")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetInventoryById([Required] string id)
    {
        var result = await _repository.GetAllByIdAsync(id);
        if (result == null) return NotFound();
        
        return Ok(result);
    }
    
    [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder([Required] string itemNo, [FromBody] PurchaseItemDto model)
    {
        var result = await _repository.PurchaseItemAsync(itemNo, model);
        return Ok(result);
    }
    
    [Route("{id}", Name = "DeleteById")]
    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteById([Required] string id)
    {
        var entity = await _repository.GetAllByIdAsync(id);
        if (entity == null) return NotFound();
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}