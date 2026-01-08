using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Infrastructure.Data
{
        public class TaskListDbContext(DbContextOptions<TaskListDbContext> options) : IdentityDbContext<ApplicationUser>(options)
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder); 

                modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskListDbContext).Assembly);
            }


            public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
            public DbSet<ToDoList> ToDoLists => Set<ToDoList>();
            public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
        }
}
