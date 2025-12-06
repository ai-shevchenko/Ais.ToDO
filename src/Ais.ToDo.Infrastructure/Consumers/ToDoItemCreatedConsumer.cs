using Ais.ToDo.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ais.ToDo.Infrastructure.Consumers;

internal sealed class ToDoItemCreatedConsumer : IConsumer<ToDoItemCreatedDto>
{
    private readonly ILogger<ToDoItemCreatedConsumer> _logger;

    public ToDoItemCreatedConsumer(ILogger<ToDoItemCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ToDoItemCreatedDto> context)
    {
        return Task.CompletedTask;
    }
}