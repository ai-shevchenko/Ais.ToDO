namespace Ais.ToDo.Contracts;

public sealed record ToDoItemDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}