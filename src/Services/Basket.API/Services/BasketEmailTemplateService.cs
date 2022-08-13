using Basket.API.Services.Interfaces;

namespace Basket.API.Services;

public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
{
    public string GenerateReminderCheckoutOrderEmail(string email, string username)
    {
        var checkoutUrl = "http://localhost:5001/baskets/checkout";
        var emailText = ReadEmailTemplateContent("reminder-checkout-order");
        return emailText.Replace("[username]", username)
            .Replace("[checkoutUrl]", checkoutUrl);
    }
}