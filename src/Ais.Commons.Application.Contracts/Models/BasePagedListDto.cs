namespace Ais.Commons.Application.Contracts.Models;

public abstract record BasePagedListDto<TListItem>
{
    public required IReadOnlyList<TListItem> Items { get; init; } = [];
    public required int TotalCount { get; init; }
}