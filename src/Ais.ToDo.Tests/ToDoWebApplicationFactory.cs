using Ais.Commons.Tests.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Ais.ToDo.Tests;

public sealed class ToDoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgresTests  _postgres;
    private readonly RabbitMqTests  _rabbitmq;
    private readonly RedisTests  _redis;
    
    public ToDoWebApplicationFactory(PostgresTests postgres, RabbitMqTests rabbitmq, RedisTests redis)
    {
        _postgres = postgres;
        _rabbitmq = rabbitmq;
        _redis = redis;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Postgres", _postgres.ConnectionString },
                { "ConnectionStrings:RabbitMq", _rabbitmq.ConnectionString },
                { "ConnectionStrings:Redis", _redis.ConnectionString },
            })
            .Build();
        builder.UseConfiguration(configuration);

        base.ConfigureWebHost(builder);
    }
}