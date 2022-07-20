using System.Reflection;
using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence.Interceptors;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    private readonly EntitySaveChangesInterceptor _entitySaveChangesInterceptor;
    public OrderContext(DbContextOptions<OrderContext> options, EntitySaveChangesInterceptor entitySaveChangesInterceptor) : base(options)
    {
        _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
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
        return await base.SaveChangesAsync(cancellationToken);
    }
}