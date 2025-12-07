using Ais.Commons.CQRS.Requests;
using Ais.Commons.CQRS.Tracing;

using MediatR;

namespace Ais.Commons.CQRS.Abstractions;

public abstract class BaseRequestHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest, IRequestMeta
{
    public async Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        var activityName = $"{DiagnosticHeaders.DefaultListenerName} {request.RequestName}";
        using var activity = DiagnosticHelpers.StartActivity(activityName,  request.EnableTracing);
        
        activity?.SetTag("request.type", request.GetType().FullName);
        activity?.SetTag("request.tracing", request.EnableTracing);
        activity?.SetTag("request.pipeline-behavior", request.EnablePipelineBehavior);
        
        await HandleAsync(request, cancellationToken);
    }
    
    protected abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequestMeta
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var activityName = $"{DiagnosticHeaders.DefaultListenerName} {request.RequestName}";
        using var activity = DiagnosticHelpers.StartActivity(activityName,  request.EnableTracing);
        
        activity?.SetTag("request.type", request.GetType().FullName);
        activity?.SetTag("request.tracing", request.EnableTracing);
        activity?.SetTag("request.pipeline-behavior", request.EnablePipelineBehavior);
        
        return await HandleAsync(request, cancellationToken);
    }
    
    protected abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}