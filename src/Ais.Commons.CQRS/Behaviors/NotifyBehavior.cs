using Ais.Commons.CQRS.Notifications;
using MediatR;

namespace Ais.Commons.CQRS.Behaviors;

public class NotifyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : IRequest<TResponse>
{
    private readonly IPublisher _publisher;

    public NotifyBehavior(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        if (response is null)
        {
            return response;
        }
        
        var notification = new NotificationWrapper<TResponse>()
        {
            Model = response,
        };
        await _publisher.Publish(notification, cancellationToken);
        
        return response;
    }
}