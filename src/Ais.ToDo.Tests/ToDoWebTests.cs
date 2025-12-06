using System.Net;
using System.Net.Http.Json;

using Ais.Commons.Tests;
using Ais.Commons.Tests.Containers;
using Ais.ToDo.Contracts;
using Ais.ToDo.Infrastructure.Contracts;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Tests;

public sealed class ToDoTests : BaseTests<ToDoWebApplicationFactory, Program>,
    IClassFixture<PostgresTests>,
    IClassFixture<RabbitMqTests>, 
    IClassFixture<RedisTests>
{
    public ToDoTests(
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
    
    [Fact]
    public async Task Post_CreateTask_ReturnCreated()
    {
        // Arrange
        var contract = new CreateToDoItemDto
        {
            Title = "Test title 11",
            Description = "Test description 11",
        };
        var request = JsonContent.Create(contract);
        
        // Act
        using var response = await Client.PostAsync("api/v1.0/todo-items/", request);
        response.EnsureSuccessStatusCode();
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Get_TasksPagedList_ShouldReturnOk()
    {
        // Arrange
        var query = new QueryBuilder
        {
            { "PageIndex", "0" }, 
            { "PageSize", "10" }, 
            { "SortField", "Id" }, 
            { "SortOrder", "Ascending" }
        };
        var queryString = query.ToQueryString().Value;
        
        // Act
        using var response = await Client.GetAsync("api/v1.0/todo-items/page" + queryString);
        response.EnsureSuccessStatusCode();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}