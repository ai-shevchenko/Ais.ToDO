using Testcontainers.RabbitMq;

namespace Ais.ToDo.Tests;

public sealed class RabbitMqTests : BaseContainerTests
{
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.11")
        .Build();
    
    public override string ConnectionString => _rabbitMq.GetConnectionString();
    
    public override async Task InitializeAsync()
    {
        await _rabbitMq.StartAsync();
    }

    public override async Task DisposeAsync()
    {
        await _rabbitMq.StopAsync();
        await _rabbitMq.DisposeAsync();
    }
}