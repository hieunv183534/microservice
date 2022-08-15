using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.ScheduledJob;
using Shared.Services;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;
    private readonly BackgroundJobHttpService _backgroundJobHttp;
    private readonly IEmailTemplateService _emailTemplateService;

    public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger, BackgroundJobHttpService backgroundJobHttp, IEmailTemplateService emailTemplateService)
    {
        _redisCacheService = redisCacheService;
        _serializeService = serializeService;
        _logger = logger;
        _backgroundJobHttp = backgroundJobHttp;
        _emailTemplateService = emailTemplateService;
    }
    
    public async Task<Cart?> GetBasketByUserName(string username)
    {
        _logger.Information($"BEGIN: GetBasketByUserName {username}");
        var basket = await _redisCacheService.GetStringAsync(username);
        _logger.Information($"END: GetBasketByUserName {username}");
        
        return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
    }

    public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
    {
        _logger.Information($"BEGIN: UpdateBasket for {cart.Username}");

        if (options != null)
            await _redisCacheService.SetStringAsync(cart.Username,
                _serializeService.Serialize(cart), options);
        else
            await _redisCacheService.SetStringAsync(cart.Username,
                _serializeService.Serialize(cart));
        
        _logger.Information($"END: UpdateBasket for {cart.Username}");
        try
        {
            await TriggerSendEmailReminderCheckout(cart);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }

        return await GetBasketByUserName(cart.Username);
    }

    private async Task TriggerSendEmailReminderCheckout(Cart cart)
    {
        var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(cart.Username);

        var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "Reminder checkout", emailTemplate,
            DateTimeOffset.UtcNow.AddSeconds(30));

        const string uri = "/api/scheduled-jobs/send-email-reminder-checkout-order";
        var response = await _backgroundJobHttp.Client.PostAsJson(uri, model);
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            var jobId = await response.ReadContentAs<string>();
            if (!string.IsNullOrEmpty(jobId))
            {
                cart.JobId = jobId;
                await _redisCacheService.SetStringAsync(cart.Username,
                    _serializeService.Serialize(cart));
            }
        }
    }
    
    public async Task<bool> DeleteBasketFromUserName(string username)
    {
        try
        {
            _logger.Information($"BEGIN: DeleteBasketFromUserName {username}");
            await _redisCacheService.RemoveAsync(username);
            _logger.Information($"END: DeleteBasketFromUserName {username}");

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("Error DeleteBasketFromUserName: " + e.Message);
            throw;
        }
    }
}