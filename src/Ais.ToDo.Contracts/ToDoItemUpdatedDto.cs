using Ais.Commons.Application.Contracts.Models;

namespace Ais.ToDo.Contracts;

public sealed record ToDoItemUpdatedDto : BaseItemCreatedDto<Guid>
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}