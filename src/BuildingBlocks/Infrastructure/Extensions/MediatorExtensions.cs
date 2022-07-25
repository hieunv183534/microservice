using Contracts.Domain.SeedWork;
using Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext context, ILogger logger)
    {
        var domainEntities = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();
        
        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();
        
        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
            var data = new SerializeService().Serialize(domainEvent);
            logger.Information($"\n-----\nA domain event has been published!\n" +
                               $"Event: {domainEvent.GetType().Name}\n" +
                               $"Data: {data})\n-----\n");
        }
    }
}