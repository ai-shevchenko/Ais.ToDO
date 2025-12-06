using Ais.Commons.EntityFramework.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ais.ToDo.Infrastructure.Postgres;

internal sealed class ToDoDatabaseMigrator : BaseDatabaseMigrator<ToDoDbDbContext>
{
    public ToDoDatabaseMigrator(ToDoDbDbContext context, ILogger<ToDoDatabaseMigrator> logger)
        : base(context, logger)
    {
    }

    public override async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Start migrating database.");
        
        var pending = (await Context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (pending.Count > 0)
        {
            Logger.LogInformation("Found next pending migrations: {@MigrationNames}.", string.Join(", ", pending));
            
            await Context.Database.MigrateAsync(cancellationToken);
            
            Logger.LogInformation("All migrations was applied.");
        }
        
        await MigrateDataAsync(cancellationToken);
        
        Logger.LogInformation("Database migrated.");
    }

    public override Task MigrateDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}