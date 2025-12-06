using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ais.Commons.EntityFramework.Contracts;

public interface IDbContext : IDisposable, IAsyncDisposable
{
    IModel Model { get; }
    
    DatabaseFacade Database { get; }
    
    ChangeTracker ChangeTracker { get; }
    
    DbSet<TEntity> Set<TEntity>() 
        where TEntity : class;
    
    DbSet<TEntity> Set<TEntity>(string name)
        where TEntity : class;
    
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}