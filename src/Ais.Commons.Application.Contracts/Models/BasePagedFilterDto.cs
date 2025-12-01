using System.ComponentModel.DataAnnotations;

namespace Ais.Commons.Application.Contracts.Models;

public abstract record BasePagedFilterDto<TItem> : BaseFilterDto<TItem>
{
    [Range(0, int.MaxValue)]
    public required int PageIndex { get; init; }
    
    [Range(0, int.MaxValue)]
    public required int PageSize { get; init; }
}