using Ais.Commons.Application.Contracts.Models;

namespace Ais.ToDo.Contracts;

public sealed record PagedToDoListDto : BasePagedListDto<PagedToDoListItemDto>;
