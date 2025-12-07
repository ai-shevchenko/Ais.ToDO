namespace Ais.Commons.CQRS.Requests;

public interface ICacheableRequest : IRequestMeta
{
    TimeSpan SlidingExpiration { get; }
    TimeSpan AbsoluteExpirationRelativeToNow { get; }
    string CacheKey { get; }
}