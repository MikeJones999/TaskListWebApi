using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;

namespace TaskList.Api.Infrastructure.Data
{
        public class TaskListDbContext(DbContextOptions<TaskListDbContext> options) : IdentityDbContext<ApplicationUser>(options)
        {
           // public DbSet<x> Xs {get; set;}
        }
}
