using Contracts.Common;
using Contracts.Domains.Interfaces;

namespace Contracts.Domain.SeedWork;

public interface IAggregateRoot : IAggregateRoot<long>
{
}

public interface IAggregateRoot<T> : IEntityBase<T>
{
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
}