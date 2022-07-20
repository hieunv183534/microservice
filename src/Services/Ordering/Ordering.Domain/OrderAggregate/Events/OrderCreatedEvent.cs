using Contracts.Common;
namespace Ordering.Domain.OrderAggregate.Events;

public class OrderCreatedEvent : BaseEvent
{
    public string UserName { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string ShippingAddress { get; private set; }
    public string InvoiceAddress { get; private set; }

    public EOrderStatus Status { get; set; }

    public OrderCreatedEvent(string userName, decimal totalPrice, 
        string shippingAddress, string invoiceAddress)
    {
        UserName = userName;
        TotalPrice = totalPrice;
        ShippingAddress = shippingAddress;
        InvoiceAddress = invoiceAddress;
    }
}