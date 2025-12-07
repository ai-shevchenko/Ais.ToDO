using Ais.Commons.CQRS.Requests;
using Ais.Commons.CQRS.Tracing;
using Ais.Commons.EntityFramework.Contracts;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Ais.Commons.CQRS.Behaviors;

public abstract class TransactionalBehavior<TContext, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TContext : IDbContext
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly TContext _context;

    protected TransactionalBehavior(TContext context)
    {
        _context = context;
    }

    public virtual async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!request.EnablePipelineBehavior || _context.Database.CurrentTransaction is not null)
        {
            return await next(cancellationToken);
        }

        var activityName = $"{DiagnosticHeaders.DefaultListenerName} TransactionalBehavior {request.RequestName}";
        using var activity = DiagnosticHelpers.StartActivity(activityName, request.EnableTracing);
        
        var strategy = _context.Database.CreateExecutionStrategy();
        var response = await strategy.ExecuteAsync(async (token) =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(token);
            
            activity?.SetTag("transaction.id", transaction.TransactionId);
            
            var response = await next(token);

            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            
            return response;
        }, cancellationToken);
        return response;
    }
}