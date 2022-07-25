using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domain.SeedWork;
using Contracts.Domains;

namespace Contracts.Common;

public abstract class AggregateRoot : AggregateRoot<long>
{
}

public abstract class AggregateRoot<T> : EntityBase<T>, IAggregateRoot<T>
{
    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}