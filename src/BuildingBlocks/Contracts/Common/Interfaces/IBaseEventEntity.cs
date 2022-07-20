namespace Contracts.Common.Interfaces;

public interface IBaseEventEntity : IBaseEventEntity<long>
{
   
}

public interface IBaseEventEntity<T>
{
    T Id { get; set; }
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
}