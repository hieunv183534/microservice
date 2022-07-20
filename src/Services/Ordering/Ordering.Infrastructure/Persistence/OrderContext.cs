using System.Reflection;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence.Interceptors;
using Serilog;
using MediatR;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly EntitySaveChangesInterceptor _entitySaveChangesInterceptor;
    private readonly ILogger _logger;
    
    public OrderContext(DbContextOptions<OrderContext> options, EntitySaveChangesInterceptor entitySaveChangesInterceptor, ILogger logger, IMediator mediator) : base(options)
    {
        _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
        _logger = logger;
        _mediator = mediator;
    }
    
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_entitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Dispatch Domain Events collection.
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.

        await _mediator.DispatchDomainEventAsync(this, _logger);
        var result = await base.SaveChangesAsync(cancellationToken);
        
        return result;
    }
}