using Ais.Commons.EntityFramework.Contracts;
using Ais.ToDo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ais.ToDo.Infrastructure.Contracts;

public interface IToDoDbContext : IDbContext
{
    DbSet<ToDoItem> ToDoItems { get; }
}