using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Infrastructure.Configurations
{
    public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
    {
        public void Configure(EntityTypeBuilder<ToDoItem> builder)
        {
            builder.HasKey(t => t.Id);

            builder
                .HasOne(t => t.ToDoList)
                .WithMany(l => l.ToDoItems)
                .HasForeignKey(t => t.ToDoListId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
