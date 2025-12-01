namespace Ais.Commons.Application.Contracts.Models;

public abstract record BaseItemCreatedDto<TId>
{
    public required TId Id { get; init; }
}