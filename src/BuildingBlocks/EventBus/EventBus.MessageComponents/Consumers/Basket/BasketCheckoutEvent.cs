using EventBus.Messages;

namespace EventBus.MessageComponents.Consumers.Basket;

/// <inheritdoc cref="EventBus.Messages.Events.IntegrationBaseEvent" />
public record BasketCheckoutEvent : IntegrationBaseEvent, IBasketCheckoutEvent
{
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
}