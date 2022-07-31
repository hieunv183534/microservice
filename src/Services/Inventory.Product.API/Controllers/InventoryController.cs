using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;

    public InventoryController(IInventoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    [Route("{itemNo}", Name = "GetInventoryByItemNo")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntry>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntry>>> GetInventoryByItemNo([Required]string itemNo)
    {
        var entities = await _repository.GetProductInventories(itemNo);
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return Ok(result);
    }
}