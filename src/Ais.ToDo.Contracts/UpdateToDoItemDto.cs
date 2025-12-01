namespace Ais.ToDo.Contracts;

public sealed record UpdateToDoItemDto : CreateToDoItemDto
{
    public required Guid Id { get; init; }
    public bool IsCompleted { get; init; }
}