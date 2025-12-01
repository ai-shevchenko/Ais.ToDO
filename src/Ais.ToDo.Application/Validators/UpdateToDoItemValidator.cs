using Ais.Commons.Utils.Extensions;
using Ais.ToDo.Contracts;
using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Application.Validators;

internal sealed class UpdateToDoItemValidator : ToDoItemValidator<UpdateToDoItemDto>
{
    public UpdateToDoItemValidator(IToDoContext context) 
        : base(context)
    {
    }
    
    protected override async Task<bool> MustUniqueTitleAsync(UpdateToDoItemDto model, string title, CancellationToken cancellationToken)
    {
        return !await Context.ToDoItems
            .Where(x => x.Id != model.Id)
            .Search(nameof(ToDoItem.Title), title)
            .AnyAsync(cancellationToken);
    }
}