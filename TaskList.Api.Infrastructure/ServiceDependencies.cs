using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskList.Api.Infrastructure.Data;
using TaskList.Api.Infrastructure.Services;

namespace TaskList.Api.Infrastructure
{
    public static class ServiceDependencies
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Add infrastructure-related services here
            services.AddDbContext<TaskListDbContext>(options => options.UseSqlite(connectionString));
            services.AddHostedService<DatabaseMigrationService>();
            return services;
        }
    }
}
