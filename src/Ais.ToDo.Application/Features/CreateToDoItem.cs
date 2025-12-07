using Ais.Commons.CQRS.Abstractions;
using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;

using AutoMapper;

using FluentValidation;

using MassTransit;


namespace Ais.ToDo.Application.Features;

public static class CreateToDoItem
{
    public sealed record Command : BaseRequest<CreateToDoItemDto, ToDoItemCreatedDto>, 
        IValidatableRequest,
        ITransactionalRequest
    {
        public Command(CreateToDoItemDto model) 
            : base(model)
        {
        }

        public override string RequestName => nameof(CreateToDoItem);
    }
    
    internal sealed class Handler : BaseRequestHandler<Command, ToDoItemCreatedDto>
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

        protected override async Task<ToDoItemCreatedDto> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var toDoItem = _mapper.Map<ToDoItem>(request.Model);
            await _context.ToDoItems.AddAsync(toDoItem, cancellationToken);
            
            var created = _mapper.Map<ToDoItemCreatedDto>(toDoItem);
            await _publishEndpoint.Publish(created, cancellationToken);
            
            await _context.SaveChangesAsync(cancellationToken);
            
            return created;
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {   
        public Validator(IValidator<CreateToDoItemDto> validator)
        {
            RuleFor(x => x.Model)
                .SetValidator(validator);
        }
    }
    
    internal sealed class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateToDoItemDto, ToDoItem>(MemberList.None);
            CreateMap<ToDoItem, ToDoItemCreatedDto>(MemberList.None);
        }
    }
}