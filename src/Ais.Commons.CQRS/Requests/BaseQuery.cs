using MediatR;

namespace Ais.Commons.CQRS.Requests;

public abstract record BaseQuery<TResult> : IRequest<TResult>;

public abstract record BaseQuery<TModel, TResult> : BaseQuery<TResult>
{
    protected BaseQuery(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Model = model;
    }
    
    public TModel Model { get; }
}