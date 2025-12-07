using Ais.Commons.CQRS.Requests;
using Ais.Commons.CQRS.Tracing;

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
        if (!request.EnablePipelineBehavior || !_validators.Any())
        {
            return await next(cancellationToken);
        }
        
        var activityName = $"{DiagnosticHeaders.DefaultListenerName} ValidationBehavior {request.RequestName}";
        using var activity = DiagnosticHelpers.StartActivity(activityName, request.EnableTracing);

        var number = 0;
        foreach (var validator in _validators)
        {
            number++;
            activity?.SetTag($"validators.{number}.type", validator.GetType().FullName);
        }
        activity?.SetTag("validators.count", number);

        var tasks =_validators
            .Select(v => v.ValidateAsync(request, cancellationToken))
            .ToList();
        
        await Task.WhenAll(tasks);

        var errors = tasks
            .Select(x => x.Result)
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .ToList();

        activity?.SetTag("validators.errors", errors.Count);

        var errorNumber = 1;
        foreach (var error in errors)
        {
            activity?.SetTag($"errors.{errorNumber}.property", error.PropertyName);
            activity?.SetTag($"errors.{errorNumber}.message", error.ErrorMessage);
            activity?.SetTag($"errors.{errorNumber}.severity", error.Severity);
            
            errorNumber++;
        }
        
        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
        
        return await next(cancellationToken);
    }
}