using EventBus.Messages.Events;

namespace Basket.API.IntegrationEvents.Events;

public record BasketCheckoutEvent : IntegrationBaseEvent
{
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }

    public BasketCheckoutEvent(string userName) => UserName = userName;
}