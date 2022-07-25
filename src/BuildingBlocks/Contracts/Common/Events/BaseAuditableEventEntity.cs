using Contracts.Domain.SeedWork;
using Contracts.Domains.Interfaces;

namespace Contracts.Common;

public abstract class BaseAuditableEventEntity : BaseAuditableEventEntity<long>
{
}

public abstract class BaseAuditableEventEntity<T> : AggregateRoot<T>, IAuditable, IAggregateRoot<T>
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}