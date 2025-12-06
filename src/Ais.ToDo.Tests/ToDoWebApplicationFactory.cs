using Ais.Commons.Tests;
using Ais.Commons.Tests.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Ais.ToDo.Tests;

public sealed class ToDoTestsApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgresTests  _postgres;
    private readonly RabbitMqTests  _rabbitmq;
    private readonly RedisTests  _redis;
    
    public ToDoTestsApplicationFactory(PostgresTests postgres, RabbitMqTests rabbitmq, RedisTests redis)
    {
        _postgres = postgres;
        _rabbitmq = rabbitmq;
        _redis = redis;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings__Postgres", _postgres.ConnectionString },
                { "ConnectionStrings__RabbitMQ", _rabbitmq.ConnectionString },
                { "ConnectionStrings__Redis", _redis.ConnectionString },
            });
        });
        
        builder.UseEnvironment("Development");
    }
}