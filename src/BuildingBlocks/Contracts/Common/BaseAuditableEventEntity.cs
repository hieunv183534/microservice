using Contracts.Domains.Interfaces;

namespace Contracts.Common;

public abstract class BaseAuditableEventEntity : BaseAuditableEventEntity<long>
{
}

public abstract class BaseAuditableEventEntity<T> : BaseEventEntity<T>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}