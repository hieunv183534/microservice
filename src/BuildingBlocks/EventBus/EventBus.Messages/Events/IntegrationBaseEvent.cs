using System.Text.Json.Serialization;

namespace EventBus.Messages.Events;

public record IntegrationBaseEvent : IIntegrationEvent
{
    public IntegrationBaseEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationBaseEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }
    
    [JsonInclude]
    public DateTime CreationDate { get; init; }

    [JsonInclude]
    public Guid Id { get; init; }
}