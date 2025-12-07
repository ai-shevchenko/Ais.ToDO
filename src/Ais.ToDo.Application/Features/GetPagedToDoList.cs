using Ais.Commons.CQRS.Abstractions;
using Ais.Commons.Utils.Extensions;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Application.Features;

public static class GetPagedToDoList
{
    public sealed record Query : BaseRequest<GetPagedToDoListDto, PagedToDoListDto>
    {
        public Query(GetPagedToDoListDto model) 
            : base(model)
        {
        }

        public override string RequestName => nameof(GetPagedToDoList);
    }
    
    internal sealed class Handler : BaseRequestHandler<Query, PagedToDoListDto>
    {
        private readonly IToDoDbContext _context;
        private readonly IMapper _mapper;
        
        public Handler(IToDoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<PagedToDoListDto> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var filter = request.Model;
            
            var query = _context.ToDoItems.AsNoTracking();

            if (filter.IsCompleted.HasValue)
            {
                query = query.Where(x => x.IsCompleted == filter.IsCompleted.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Search(filter.SearchableFields, filter.Search);
            }
            
            var total = await query.CountAsync(cancellationToken);
            
            query = query.Sort(filter.SortField, filter.SortOrder);
            
            var items = await query
                .Page(filter.PageIndex, filter.PageSize)
                .ProjectTo<PagedToDoListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PagedToDoListDto
            {
                TotalCount = total,
                Items = items
            };
        }
    }
    
    internal sealed class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ToDoItem, PagedToDoListItemDto>(MemberList.None);
        }
    }
}