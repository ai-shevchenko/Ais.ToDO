using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;

using MediatR;

using Microsoft.Extensions.Caching.Distributed;

namespace Ais.ToDo.Application.Features;

public static class InvalidateToDoItemCache
{
    public sealed record Command : BaseCommand<ToDoItemUpdatedDto>
    {
        public Command(ToDoItemUpdatedDto model) 
            : base(model)
        {
        }
        
        public string CacheKey => $"todo-item:{Model.Id}";
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
            await _cache.RemoveAsync(request.CacheKey, cancellationToken);
        }
    }
}