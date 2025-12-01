using Ais.Commons.CQRS.Requests;
using FluentValidation;
using MediatR;

namespace Ais.Commons.CQRS.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidatableRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var tasks =_validators
            .Select(v => v.ValidateAsync(request, cancellationToken))
            .ToList();
        
        await Task.WhenAll(tasks);

        var errors = tasks
            .Select(x => x.Result)
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .ToList();

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
        
        return await next(cancellationToken);
    }
}