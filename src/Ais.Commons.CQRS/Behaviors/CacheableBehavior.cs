using System.Text.Json;
using Ais.Commons.CQRS.Requests;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ais.Commons.CQRS.Behaviors;

public sealed class CacheableBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : IRequest<TResponse>, ICacheableRequest
{
    private readonly IDistributedCache _cache;

    public CacheableBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
    {
        var key = request.CacheKey;
        var value = await _cache.GetStringAsync(key, cancellationToken);

        TResponse? response;
        
        if (string.IsNullOrWhiteSpace(value))
        {
            response = await next(cancellationToken);
            var json = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = request.SlidingExpiration,
                    AbsoluteExpirationRelativeToNow = request.AbsoluteExpirationRelativeToNow
                }, 
                cancellationToken);
            
            return response;
        }
        
        response = JsonSerializer.Deserialize<TResponse>(value);
        return response;
    }
}