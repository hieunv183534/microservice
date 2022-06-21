using Microsoft.EntityFrameworkCore;

namespace Contracts.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}

public interface IUnitOfWork<TContext> : IUnitOfWork, IDisposable where TContext : DbContext
{
}