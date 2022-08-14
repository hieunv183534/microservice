using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;

namespace Basket.API.Services;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    private readonly ISerializeService _serializeService;

    public BasketEmailTemplateService(ISerializeService serializeService)
    {
        _serializeService = serializeService;
    }
    
    public string GenerateReminderCheckoutOrderEmail(string email, string username)
    {
        var checkoutUrl = "http://localhost:5001/baskets/checkout";
        var emailText = ReadEmailTemplateContent("reminder-checkout-order");
        var emailReplacedText = emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", checkoutUrl);
        return emailReplacedText;
        // return _serializeService.Serialize(emailReplacedText);
    }
}