using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Constraints;
using Ais.ToDo.Infrastructure.Contracts;
using FluentValidation;

namespace Ais.ToDo.Application.Validators;

internal abstract class ToDoItemValidator<T> : AbstractValidator<T>
    where T : CreateToDoItemDto
{
    protected readonly IToDoDbContext Context;
            
    protected ToDoItemValidator(IToDoDbContext context)
    {
        Context = context;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(ToDoItemConstraints.TitleMaxLength)
            .MustAsync(MustUniqueTitleAsync);

        RuleFor(x => x.Description)
            .MaximumLength(ToDoItemConstraints.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    protected abstract Task<bool> MustUniqueTitleAsync(T model, string title, CancellationToken cancellationToken);
}