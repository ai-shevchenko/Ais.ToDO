namespace Ais.Commons.CQRS.Requests;

public interface ICacheableRequest
{
    TimeSpan SlidingExpiration { get; }
    TimeSpan AbsoluteExpirationRelativeToNow { get; }
    string CacheKey { get; }
}