using Ais.Commons.Application.Contracts.Models;

namespace Ais.ToDo.Contracts;

public sealed record GetPagedToDoListDto : BasePagedFilterDto<PagedToDoListItemDto>
{
    public bool? IsCompleted { get; init; }
}