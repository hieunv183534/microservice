using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;

namespace Contracts.Common.Events;

public abstract class AuditableEventEntity<T> : EventEntity<T>, IAuditable, IEventEntity<T>
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}