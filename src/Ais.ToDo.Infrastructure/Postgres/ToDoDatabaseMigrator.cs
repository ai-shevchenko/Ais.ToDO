using Ais.Commons.EntityFramework.Contracts;
using Ais.ToDo.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ais.ToDo.Infrastructure.Postgres;

internal sealed class ToDoDatabaseMigrator : IDatabaseMigrator
{
    private readonly IToDoContext _context;
    private readonly ILogger<ToDoDatabaseMigrator> _logger;

    public ToDoDatabaseMigrator(IToDoContext context, ILogger<ToDoDatabaseMigrator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Migrating database...");
        
        var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
        {
            await _context.Database.MigrateAsync(cancellationToken);
        }
        
        await MigrateDataAsync(cancellationToken);
        
        _logger.LogInformation("Database migrated.");
    }

    public Task MigrateDataAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}