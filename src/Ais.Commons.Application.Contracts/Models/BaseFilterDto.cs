using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ais.Commons.Utils.Models;

namespace Ais.Commons.Application.Contracts.Models;

public abstract record BaseFilterDto<TResult> : IValidatableObject
{
    public required string SortField { get; init; }
    public IReadOnlyList<string> SearchableFields { get; init; } = [];
    public required SortOrder SortOrder { get; init; }
    public string? Search { get; init; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var searchableField in SearchableFields)
        {
            var searchableProperty = typeof(TResult)
                .GetProperty(searchableField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (searchableProperty == null)
            {
                yield return new ValidationResult(
                    $"Property '{searchableField}' not found on type '{typeof(TResult)}'.",
                    [nameof(SearchableFields)]);
                
                continue;
            }

            if (searchableProperty.PropertyType != typeof(string))
            {
                yield return new ValidationResult(
                    $"Property '{searchableField}' not a '{typeof(string)}' type.",
                    [nameof(SearchableFields)]);
            }
        }
        
        var property = typeof(TResult)
            .GetProperty(SortField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (property is null)
        {
            yield return new ValidationResult(
                $"Property '{SortField}' not found on type '{typeof(TResult)}'.",
                [nameof(SortField)]);
        }
    }
}