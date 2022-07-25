using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Common;
using Contracts.Common.Interfaces;
using Contracts.Domain.SeedWork;
using Ordering.Domain.OrderAggregate.Events;

namespace Ordering.Domain.Entities;

public class Order : BaseAuditableEventEntity<long>, IAggregateRoot
{
    [Required]
    [Column(TypeName = "nvarchar(150)")]
    public string UserName { get; set; }

    public Guid DocumentNo { get; set; } = Guid.NewGuid();

    [Column(TypeName = "decimal(10,2)")] 
    public decimal TotalPrice { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(250)")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Column(TypeName = "nvarchar(250)")]
    public string EmailAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")] 
    public string ShippingAddress { get; set; }
    
    [Column(TypeName = "nvarchar(max)")] 
    public string InvoiceAddress { get; set; }

    public EOrderStatus Status { get; set; }

    [NotMapped]
    public string FullName => FirstName + " " + LastName;

    public Order AddedOrder()
    {
        AddDomainEvent(new OrderCreatedEvent(id: Id, userName: UserName, emailAddress: EmailAddress, fullName: FullName, totalPrice: TotalPrice,
            shippingAddress: ShippingAddress,
            invoiceAddress: InvoiceAddress, DocumentNo.ToString()));
        return this;
    }
    
    public Order DeletedOrder()
    {
        AddDomainEvent(new OrderDeletedEvent(id: Id));
        return this;
    }
}