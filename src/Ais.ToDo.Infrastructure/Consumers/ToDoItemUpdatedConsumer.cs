using Ais.ToDo.Application.Features;
using Ais.ToDo.Contracts;
using MassTransit;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Ais.ToDo.Infrastructure.Consumers;

internal sealed class ToDoItemUpdatedConsumer : IConsumer<ToDoItemUpdatedDto>
{
    private readonly ILogger<ToDoItemUpdatedConsumer> _logger;
    private readonly ISender _sender; 
    
    public ToDoItemUpdatedConsumer(ILogger<ToDoItemUpdatedConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ToDoItemUpdatedDto> context)
    {
        var command = new InvalidateToDoItemCache.Command(context.Message);
        await _sender.Send(command, context.CancellationToken);
    }
}