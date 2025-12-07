using Ais.Commons.CQRS.Requests;

using MediatR;

namespace Ais.Commons.CQRS.Abstractions;

public abstract record BaseRequest<TModel> : IRequest, IRequestMeta
    where TModel : class
{
    protected BaseRequest(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Model = model;
    }
    
    public TModel Model { get; }

    public virtual string RequestName => GetType().Name;
    public virtual bool EnableTracing { get; } = true;
    public virtual bool EnablePipelineBehavior { get; } = true;
}

public abstract record BaseRequest<TModel, TResponse> : IRequest<TResponse>, IRequestMeta
    where TModel : class
{
    protected BaseRequest(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Model = model;
    }
    
    public TModel Model { get; }
    
    public virtual string RequestName => GetType().Name;
    public virtual bool EnableTracing { get; } = true;
    public virtual bool EnablePipelineBehavior { get; } = true;
}