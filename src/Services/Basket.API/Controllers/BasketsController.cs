using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.ScheduledJob;
using Shared.Services;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly StockItemGrpcService _stockItemGrpcService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly BackgroundJobHttpService _backgroundJobHttp;

    public BasketsController(IBasketRepository basketRepository, IMapper mapper, IPublishEndpoint publishEndpoint, StockItemGrpcService stockItemGrpcService, IEmailTemplateService emailTemplateService, BackgroundJobHttpService backgroundJobHttp)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
        _emailTemplateService = emailTemplateService;
        _backgroundJobHttp = backgroundJobHttp;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> GetBasket([Required] string username)
    {
        var result = await _basketRepository.GetBasketByUserName(username);

        return Ok(result ?? new Cart(username));
    }
    
    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> UpdateBasket([FromBody] Cart cart)
    {
        // Communicate with Inventory.Product.Grpc and check quantity available of products
        foreach (var item in cart.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            item.SetAvailableQuantity(stock.Quantity);
        }
        
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
        //     .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        
        var result = await _basketRepository.UpdateBasket(cart, options);
        return Ok(result);
    }
    
    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
    {
        var result = await _basketRepository.DeleteBasketFromUserName(username);
        return Ok(result);
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketByUserName(basketCheckout.UserName);
        if (basket == null || !basket.Items.Any()) return NotFound();
        
        //publish checkout event to EventBus Message
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);
        //remove the basket
        await _basketRepository.DeleteBasketFromUserName(basket.Username);
        
        return Accepted();
    }

    [HttpPost("[action]",Name = "SendEmailReminder")]
    [ProducesResponseType(typeof(ContentResult), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult<Cart>> SendEmailReminder()
    {
        var email = "yegammoissute-7722@yopmail.com";
        var username = "yegammoissute";
        var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(email, username);

        var model = new ReminderCheckoutOrderDto(email, "reminder checkout", emailTemplate,
            DateTimeOffset.UtcNow.AddSeconds(30));
        
        var uri = "/api/scheduled-jobs/send-email-reminder-checkout-order";
        var response = await _backgroundJobHttp.Client.PostAsJsonAsync(uri, model);
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            var result = await response.ReadContentAs<string>();
            return Ok($"JobId: {result}");
        }

        return NoContent();
    }
}