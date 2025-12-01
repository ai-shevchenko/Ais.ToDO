namespace Ais.Commons.Application.Contracts.Models;

public sealed record EnumDto<TEnum>
    where TEnum : struct, Enum
{
    public required TEnum Value { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; } 
}