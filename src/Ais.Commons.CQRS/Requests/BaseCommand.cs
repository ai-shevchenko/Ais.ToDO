using MediatR;

namespace Ais.Commons.CQRS.Requests;

public abstract record BaseCommand<TModel> : IRequest
{
    protected BaseCommand(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Model = model;
    }

    public TModel Model { get; }
}

public abstract record BaseCommand<TModel, TResult> : IRequest<TResult>
{
    protected BaseCommand(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Model = model;
    }

    public TModel Model { get; }
}