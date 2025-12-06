using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using AutoMapper;
using FluentValidation;

using MassTransit;

using MediatR;

namespace Ais.ToDo.Application.Features;

public static class UpdateToDoItem
{
    public sealed record Command : BaseCommand<UpdateToDoItemDto, ToDoItemUpdatedDto?>, 
        IValidatableRequest, 
        ITransactionalRequest
    {
        public Command(UpdateToDoItemDto model) 
            : base(model)
        {
        }
    }

    internal sealed class Handler : IRequestHandler<Command, ToDoItemUpdatedDto?>
    {
        private readonly IToDoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public Handler(IToDoDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ToDoItemUpdatedDto?> Handle(Command request, CancellationToken cancellationToken)
        {
            var item = await _context.ToDoItems
                .FindAsync([request.Model.Id], cancellationToken: cancellationToken);

            if (item is null)
            {
                return null;
            }

            item = _mapper.Map(request.Model, item);

            var updated = _mapper.Map<ToDoItemUpdatedDto>(item);
            await _publishEndpoint.Publish(updated, cancellationToken);
            
            await _context.SaveChangesAsync(cancellationToken);
            
            return updated;
        }
    }
    
    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator(IValidator<UpdateToDoItemDto>  validator)
        {
            RuleFor(x => x.Model)
                .SetValidator(validator);
        }
    }
    
    internal sealed class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UpdateToDoItemDto, ToDoItem>(MemberList.None);
            CreateMap<ToDoItem, ToDoItemUpdatedDto>(MemberList.None);
        }        
    }
}