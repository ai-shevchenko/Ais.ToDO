using Ais.Commons.Tests;
using Ais.Commons.Tests.Containers;
using Ais.Commons.Utils.Models;
using Ais.ToDo.Application.Features;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;

using AutoFixture;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Tests;

public sealed class ToDoHandlersTests : BaseTests<ToDoWebApplicationFactory, Program>,
    IClassFixture<PostgresTests>,
    IClassFixture<RabbitMqTests>,
    IClassFixture<RedisTests>
{
    private readonly IFixture _fixture = new Fixture();
    
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
            await context.Database.MigrateAsync();
            await context.ToDoItems.AddRangeAsync(ToDoFixtures.GetItems(100));
            await context.SaveChangesAsync();
        });
    }

    public override async Task DisposeAsync()
    {
        await ExecuteAsync<IToDoDbContext>(async context =>
        {
            await context.ToDoItems.ExecuteDeleteAsync();
        });
    }

    [Fact]
    public async Task Create_ToDoItem_ShouldCreatedSuccessfully()
    {
        // Arrange
        var model = _fixture.Create<CreateToDoItemDto>();

        // Act
        var command = new CreateToDoItem.Command(model);
        var response = await SendAsync(command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(model.Title, response.Title);
        Assert.Equal(model.Description, response.Description);
    }

    [Fact]
    public async Task Update_ToDoItem_ShouldUpdatedSuccessfully()
    {
        // Arrange
        var item = await ExecuteAsync<IToDoDbContext, ToDoItem>(async context => await context.ToDoItems.FirstAsync());
        var model = _fixture.Build<UpdateToDoItemDto>()
            .With(x => x.Id, item.Id)
            .Create();
        
        // Act
        var command = new UpdateToDoItem.Command(model);
        var response = await SendAsync(command);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(model.Id, response.Id);
        Assert.Equal(model.Title, response.Title);
        Assert.Equal(model.Description, response.Description);
        Assert.Equal(model.IsCompleted, response.IsCompleted);
    }

    [Fact]
    public async Task  Get_ToDoItemPageList_ShouldReturnPagedToDoItems()
    {
        // Arrange
        var model = new GetPagedToDoListDto
        {
            PageIndex = 0,
            PageSize = 10,
            SortField = nameof(PagedToDoListItemDto.Id),
            SortOrder = SortOrder.Ascending
        };
        
        // Act
        var query = new GetPagedToDoList.Query(model);
        var response = await SendAsync(query);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(10, response.Items.Count);
    }

    [Fact]
    public async Task Get_ToDoItemById_ShouldReturnToDoItem()
    {
        // Arrange
        var item = await ExecuteAsync<IToDoDbContext, ToDoItem>(async context => await context.ToDoItems.FirstAsync());
        
        // Act
        var query = new GetToDoItemById.Request(item.Id);
        var response = await SendAsync(query);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(item.Id, response.Id);
        Assert.Equal(item.Title, response.Title);
        Assert.Equal(item.Description, response.Description);
        Assert.Equal(item.IsCompleted, response.IsCompleted);
    }
    
    private async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return await ExecuteAsync<ISender, TResponse>(async sender => await sender.Send(request));
    }
}