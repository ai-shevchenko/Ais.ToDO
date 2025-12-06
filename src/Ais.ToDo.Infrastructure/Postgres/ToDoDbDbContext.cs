using Ais.ToDo.Core.Entities;
using Ais.ToDo.Infrastructure.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ais.ToDo.Infrastructure.Postgres;

internal sealed class ToDoDbContext : DbContext, IToDoContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ToDoItem> ToDoItems { get; private init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("ais");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoDbContext).Assembly);
        modelBuilder.AddTransactionalOutboxEntities();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await Database.BeginTransactionAsync(cancellationToken);
}