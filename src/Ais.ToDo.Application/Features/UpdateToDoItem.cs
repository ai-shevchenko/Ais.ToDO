using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ais.ToDo.Application.Features;

public static class UpdateToDoItem
{
    public sealed record Command : BaseCommand<UpdateToDoItemDto, ToDoItemUpdatedDto?>, 
        IValidatableRequest, 
        ITransactionalRequest,
        IPublishableRequest,
        INotifiableRequest
    {
        public Command(UpdateToDoItemDto model) 
            : base(model)
        {
        }
    }

    internal sealed class Handler : IRequestHandler<Command, ToDoItemUpdatedDto?>
    {
        private readonly IToDoContext _context;
        private readonly IMapper _mapper;
        
        public Handler(IToDoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            await _context.SaveChangesAsync(cancellationToken);

            var updated = _mapper.Map<ToDoItemUpdatedDto>(item);
            
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