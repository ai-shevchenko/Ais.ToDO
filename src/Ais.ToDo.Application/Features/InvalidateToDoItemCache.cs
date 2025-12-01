using Ais.Commons.CQRS.Notifications;
using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ais.ToDo.Application.Features;

public static class InvalidateToDoItemCache
{
    public sealed record Command : BaseCommand<UpdateToDoItemDto>
    {
        public Command(UpdateToDoItemDto model) 
            : base(model)
        {
        }
    }
    
    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly IDistributedCache _cache;

        public Handler(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var key = $"todo-item:{request.Model.Id}";
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
    
    internal sealed class NotificationHandler : INotificationHandler<NotificationWrapper<UpdateToDoItemDto>>
    {
        private readonly IDistributedCache _cache;

        public NotificationHandler(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task Handle(NotificationWrapper<UpdateToDoItemDto> notification, CancellationToken cancellationToken)
        {
            var key = $"todo-item:{notification.Model.Id}";
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}