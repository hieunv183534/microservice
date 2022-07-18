
namespace EventBus.Messages.Events;

public interface IIntegrationEvent
{
    Guid Id { get; init; }
    DateTime CreationDate { get; init; }
}