using Basket.API.Services.Interfaces;
using Shared.Configurations;

namespace Basket.API.Services;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    public BasketEmailTemplateService(BackgroundJobSettings settings) : base(settings)
    {
    }
    
    public string GenerateReminderCheckoutOrderEmail(string username)
    {
        var _checkoutUrl = $"{BackgroundJobSettings.CheckoutUrl}/{BackgroundJobSettings.BasketUrl}/{username}";
        var emailText = ReadEmailTemplateContent("reminder-checkout-order");
        var emailReplacedText = emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", _checkoutUrl);

        return emailReplacedText;
    }

   
}