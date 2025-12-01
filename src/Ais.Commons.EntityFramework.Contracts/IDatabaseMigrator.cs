namespace Ais.Commons.EntityFramework.Contracts;

public interface IDatabaseMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
    Task MigrateDataAsync(CancellationToken cancellationToken = default);
}