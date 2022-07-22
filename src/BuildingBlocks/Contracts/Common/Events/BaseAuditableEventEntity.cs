using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;

namespace Contracts.Common;

public abstract class BaseAuditableEventEntity : BaseAuditableEventEntity<long>, IBaseEventEntity<long>
{
}

public abstract class BaseAuditableEventEntity<T> : BaseEventEntity<T>, IAuditable, IBaseEventEntity<T>
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}