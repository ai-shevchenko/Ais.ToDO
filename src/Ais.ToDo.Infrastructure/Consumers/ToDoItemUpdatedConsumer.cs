using Ais.ToDo.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ais.ToDo.Infrastructure.Consumers;

internal sealed class ToDoItemUpdatedConsumer : IConsumer<ToDoItemUpdatedDto>
{
    private readonly ILogger<ToDoItemUpdatedConsumer> _logger;

    public ToDoItemUpdatedConsumer(ILogger<ToDoItemUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ToDoItemUpdatedDto> context)
    {
        return Task.CompletedTask;
    }
}