using Ais.Commons.CQRS.Behaviors;
using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Infrastructure.Contracts;
using MediatR;

namespace Ais.ToDo.Application.Behaviors;

internal sealed class TransactionalBehavior<TRequest, TResponse> : TransactionalBehavior<IToDoDbContext, TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    public TransactionalBehavior(IToDoDbContext context) 
        : base(context)
    {
    }
}