namespace Basket.API.Services.Interfaces;

public interface IEmailTemplateService
{
    string GenerateReminderCheckoutOrderEmail(string username, string checkoutUrl = "/basket/checkout");
}