using Ais.Commons.Tests;
using Ais.Commons.Tests.Containers;
using Ais.ToDo.Infrastructure.Contracts;

using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Tests;

public sealed class ToDoHandlersTests : BaseTests<ToDoWebApplicationFactory, Program>,
    IClassFixture<PostgresTests>,
    IClassFixture<RabbitMqTests>,
    IClassFixture<RedisTests>
{
    public ToDoHandlersTests(
        PostgresTests postgres,
        RabbitMqTests rabbitmq,
        RedisTests redis)
        : base(new ToDoWebApplicationFactory(postgres, rabbitmq, redis))
    {
    }

    public override async Task InitializeAsync()
    {
        await ExecuteAsync<IToDoDbContext>(async context =>
        {
            await context.Database
                .MigrateAsync();

            await context.ToDoItems
                .AddRangeAsync(ToDoFixtures.GetItems(100));

            await context.SaveChangesAsync();
        });
    }

    public override async Task DisposeAsync()
    {
        await ExecuteAsync<IToDoDbContext>(async context =>
        {
            await context.ToDoItems
                .ExecuteDeleteAsync();
        });
    }
}