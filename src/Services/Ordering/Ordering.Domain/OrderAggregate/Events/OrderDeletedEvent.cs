using Contracts.Common;

namespace Ordering.Domain.OrderAggregate.Events;

public class OrderDeletedEvent : BaseEvent
{
    public long Id { get; }

    public OrderDeletedEvent(long id)
    {
        Id = id;
    }
}