using Ais.Commons.Application.Contracts.Models;

namespace Ais.ToDo.Contracts;

public sealed record ToDoItemCreatedDto : BaseItemCreatedDto<Guid>
{
    public required string Title { get; init; }
    public string? Description { get; init; }
}