using Ais.Commons.CQRS.Abstractions;

using Ais.ToDo.Contracts;

using Microsoft.Extensions.Caching.Distributed;

namespace Ais.ToDo.Application.Features;

public static class InvalidateToDoItemCache
{
    public sealed record Command : BaseRequest<ToDoItemUpdatedDto>
    {
        public Command(ToDoItemUpdatedDto model) 
            : base(model)
        {
        }
        
        public string CacheKey => $"todo-item:{Model.Id}";

        public override string RequestName =>  nameof(InvalidateToDoItemCache);
    }
    
    internal sealed class Handler : BaseRequestHandler<Command>
    {
        private readonly IDistributedCache _cache;

        public Handler(IDistributedCache cache)
        {
            _cache = cache;
        }

        protected override async Task HandleAsync(Command request, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(request.CacheKey, cancellationToken);
        }
    }
}