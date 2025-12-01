using MassTransit;
using MediatR;

namespace Ais.Commons.CQRS.Behaviors;

public sealed class PublishBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PublishBehavior(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        if (response is not null)
        {
            await _publishEndpoint.Publish(response, cancellationToken);
        }

        return response;
    }
}