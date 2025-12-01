using Ais.Commons.Utils.Extensions;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Application.Validators;

internal sealed class CreateToDoItemValidator : ToDoItemValidator<CreateToDoItemDto>
{
    public CreateToDoItemValidator(IToDoContext context) 
        : base(context)
    {
    }

    protected override async Task<bool> MustUniqueTitleAsync(CreateToDoItemDto model, string title, CancellationToken cancellationToken)
    {
        return !await Context.ToDoItems
            .Search(nameof(ToDoItem.Title), title)
            .AnyAsync(cancellationToken);
    }
}