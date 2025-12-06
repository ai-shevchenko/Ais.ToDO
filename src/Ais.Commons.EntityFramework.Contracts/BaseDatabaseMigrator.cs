using Microsoft.Extensions.Logging;

namespace Ais.Commons.EntityFramework.Contracts;

public abstract class BaseDatabaseMigrator<TContext> : IDatabaseMigrator
    where TContext : IDbContext
{
    protected readonly TContext Context;
    protected readonly ILogger Logger;
    
    protected BaseDatabaseMigrator(TContext context, ILogger logger)
    {
        Context = context;
        Logger = logger;
    }

    public abstract Task MigrateAsync(CancellationToken cancellationToken = default);

    public abstract Task MigrateDataAsync(CancellationToken cancellationToken = default);
}