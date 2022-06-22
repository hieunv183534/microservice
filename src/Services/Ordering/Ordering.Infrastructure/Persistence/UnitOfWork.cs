using Contracts.Common.Interfaces;

namespace Ordering.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly OrderContext _context;
    
    public UnitOfWork(OrderContext context)
    {
        _context = context;
    }
    
    public void Dispose() => _context.Dispose();

    public Task<int> CommitAsync() => _context.SaveChangesAsync();
}