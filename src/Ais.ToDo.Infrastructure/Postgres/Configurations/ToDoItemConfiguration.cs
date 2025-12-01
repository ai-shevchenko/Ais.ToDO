using Ais.ToDo.Core.Constraints;
using Ais.ToDo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ais.ToDo.Infrastructure.Postgres.Configurations;

internal sealed class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(ToDoItemConstraints.TitleMaxLength);
        
        builder.Property(x => x.Description)
            .HasMaxLength(ToDoItemConstraints.DescriptionMaxLength);
    }
}