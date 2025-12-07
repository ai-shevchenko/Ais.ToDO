using Ais.Commons.CQRS.Abstractions;
using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Application.Features;

public static class GetToDoItemById
{
    public sealed record Request : BaseRequest<GetToDoItemByIdDto, ToDoItemDto?>, ICacheableRequest
    {
        public Request(Guid id)
            : base(new GetToDoItemByIdDto { Id = id })
        {
        }
        
        public Request(GetToDoItemByIdDto model) 
            : base(model)
        {
        }

        public TimeSpan SlidingExpiration { get; } = TimeSpan.FromSeconds(30);
        public TimeSpan AbsoluteExpirationRelativeToNow { get; } = TimeSpan.FromMinutes(5);
        public string CacheKey => $"todo-item:{Model.Id}";

        public override string RequestName => nameof(GetToDoItemById);
    }
    
    internal sealed class Handler : BaseRequestHandler<Request, ToDoItemDto?>
    {
        private readonly IToDoDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IToDoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<ToDoItemDto?> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var item = await _context.ToDoItems
                .FirstOrDefaultAsync(x => x.Id == request.Model.Id, cancellationToken);

            return _mapper.Map<ToDoItemDto>(item);
        }
    }
    
    internal sealed class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ToDoItem, ToDoItemDto>();
        }
    }
}