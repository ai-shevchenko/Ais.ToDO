using Ais.Commons.CQRS.Requests;
using Ais.Commons.EntityFramework.Contracts;
using MediatR;

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
        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}