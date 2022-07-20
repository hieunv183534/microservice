using MediatR;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;

namespace Ordering.Application.Features.V1.Orders;

public class OrdersDomainHandler : 
    INotificationHandler<OrderCreatedEvent>,
    INotificationHandler<OrderDeletedEvent>
{
    private readonly ILogger _logger;

    public OrdersDomainHandler(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Send email
        
        _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }

    public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}