using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ais.ToDo.Infrastructure.Postgres;

internal sealed class ToDoContext : DbContext, IToDoContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options)
        : base(options)
    {
    }
    
    public DbSet<ToDoItem> ToDoItems { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("ais");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoContext).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await Database.BeginTransactionAsync(cancellationToken);
}