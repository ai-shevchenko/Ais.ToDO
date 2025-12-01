using System.Runtime.CompilerServices;
using Ais.Commons.DependencyInjection;
using Ais.Commons.EntityFramework.Contracts;
using Ais.ToDo.Infrastructure.Consumers;
using Ais.ToDo.Infrastructure.Contracts;
using Ais.ToDo.Infrastructure.Postgres;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Ais.ToDo.Tests")]
namespace Ais.ToDo.Infrastructure;

public sealed class InfrastructureModule : Module
{
    private const string PostgresConnectionStringName = "Postgres";
    private const string RabbitMqConnectionStringName = "RabbitMq";
    private const string RedisConnectionStringName = "Redis";
    
    public override void Configure(IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddMassTransit(services, configuration);
        AddRedis(services, configuration);
    }

    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(RedisConnectionStringName);
        services.AddStackExchangeRedisCache(options =>
        {   
            options.Configuration = connectionString;
            options.InstanceName = RedisConnectionStringName;
        });
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(RabbitMqConnectionStringName);
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();            
            
            x.AddConsumer<ToDoItemUpdatedConsumer>();
            x.AddConsumer<ToDoItemCreatedConsumer>();
            
            x.AddEntityFrameworkOutbox<ToDoContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
            
            x.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseEntityFrameworkOutbox<ToDoContext>(context);
            });
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;
                cfg.Host(connectionString);
                
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(PostgresConnectionStringName);
        services.AddDbContextPool<IToDoContext, ToDoContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ToDoContext).Assembly);
            });
        });
        services.AddTransient<IDatabaseMigrator, ToDoDatabaseMigrator>();
    }
}