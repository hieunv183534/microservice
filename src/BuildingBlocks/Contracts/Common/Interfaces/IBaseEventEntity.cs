using Contracts.Domains.Interfaces;

namespace Contracts.Common.Interfaces;

public interface IBaseEventEntity : IBaseEventEntity<long>
{
   
}

public interface IBaseEventEntity<T> : IEntityBase<T>
{
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
}