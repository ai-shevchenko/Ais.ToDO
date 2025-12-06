using Ais.Commons.CQRS.Requests;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Application.Features;

public static class GetToDoItemById
{
    public sealed record Query : BaseQuery<GetToDoItemByIdDto, ToDoItemDto?>, ICacheableRequest
    {
        public Query(Guid id)
            : base(new GetToDoItemByIdDto { Id = id })
        {
        }
        
        public Query(GetToDoItemByIdDto model) 
            : base(model)
        {
        }

        public TimeSpan SlidingExpiration { get; } = TimeSpan.FromSeconds(30);
        public TimeSpan AbsoluteExpirationRelativeToNow { get; } = TimeSpan.FromMinutes(5);
        public string CacheKey => $"todo-item:{Model.Id}";
    }
    
    internal sealed class Handler : IRequestHandler<Query, ToDoItemDto?>
    {
        private readonly IToDoDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IToDoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ToDoItemDto?> Handle(Query request, CancellationToken cancellationToken)
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