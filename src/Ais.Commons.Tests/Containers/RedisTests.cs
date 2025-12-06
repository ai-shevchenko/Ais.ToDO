using Testcontainers.Redis;

namespace Ais.Commons.Tests.Containers;

public sealed class RedisTests : BaseContainerTests
{
    private readonly RedisContainer _redis = new RedisBuilder()
        .Build();

    public override string ConnectionString => _redis.GetConnectionString();

    public override async Task InitializeAsync()
    {
        await _redis.StartAsync();
    }

    public override async Task DisposeAsync()
    {
        await _redis.StopAsync();
        await _redis.DisposeAsync();
    }
}