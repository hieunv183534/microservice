using Contracts.Common;
namespace Ordering.Domain.OrderAggregate.Events;

public class OrderCreatedEvent : BaseEvent
{
    public long Id { get; private set; }
    public string UserName { get; private set; }
    
    public string DocumentNo { get; private set; }
    public string EmailAddress { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string ShippingAddress { get; private set; }
    public string InvoiceAddress { get; private set; }
    public string FullName { get; private set; }

    public OrderCreatedEvent(long id, string userName, string emailAddress, string fullName, decimal totalPrice, 
        string shippingAddress, string invoiceAddress, string documentNo)
    {
        Id = id;
        UserName = userName;
        EmailAddress = emailAddress;
        FullName = fullName;
        TotalPrice = totalPrice;
        ShippingAddress = shippingAddress;
        InvoiceAddress = invoiceAddress;
        DocumentNo = documentNo;
    }
}