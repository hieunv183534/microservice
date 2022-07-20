using Contracts.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext context, ILogger logger)
    {
        // Message Broker for events or using MediaR

        var domainEntities = context.ChangeTracker.Entries();

        // var domainEntities = context.ChangeTracker
        //     .Entries<IBaseEventEntity>()
        //     .Where(x => x.Entity.DomainEvents.Any())
        //     .Select(x => x.Entity);

        foreach (var item in domainEntities)
        {
            if (item.Entity is IBaseEventEntity eventEntity)
            {
                // eventEntity.ClearDomainEvents();
                var domainEvents = eventEntity.DomainEvents.ToList();
                foreach (var domainEvent in domainEvents)
                {
                    await mediator.Publish(domainEvent);
                    logger.Information($"\n-----\nA domain event has been published!\n" +
                                       $"Event: {domainEvent.GetType().Name}\n" +
                                       $"Data: {JsonConvert.SerializeObject(domainEvent)}\n-----\n");
                }
            }
        }
    }
}