using Ais.Commons.Core;

namespace Ais.ToDo.Core.Entities;

public class ToDoItem : BaseEntity<Guid>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}