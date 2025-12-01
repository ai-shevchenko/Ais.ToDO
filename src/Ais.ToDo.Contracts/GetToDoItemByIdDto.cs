namespace Ais.ToDo.Contracts;

public sealed record GetToDoItemByIdDto
{
    public required Guid Id { get; init; }
}