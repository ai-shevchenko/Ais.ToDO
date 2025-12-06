using Testcontainers.PostgreSql;

namespace Ais.ToDo.Tests;

public sealed class PostgresTests : BaseContainerTests
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();
    
    public override string ConnectionString => _postgres.GetConnectionString();
    
    public override async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public override async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await  _postgres.DisposeAsync();
    }
}