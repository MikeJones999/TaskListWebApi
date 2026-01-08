using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Domain.Tasks.Interfaces.Repositories;
using TaskList.Api.Domain.Users.Interfaces.Repositories;
using TaskList.Api.Infrastructure.Data;
using TaskList.Api.Infrastructure.Repositories;
using TaskList.Api.Infrastructure.Services;
using TaskList.Api.Infrastructure.Services.FileStorage;

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

            GetFileStorageServiceByAppsettings(configuration, services);

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IToDoListRepository, ToDoListRepository>();
            services.AddTransient<IToDoItemRepository, ToDoItemRepository>();

            return services;
        }

        private static void GetFileStorageServiceByAppsettings(IConfiguration configuration, IServiceCollection services)
        {
            bool useBlobStorage = configuration.GetValue<bool>("FileStorage:UseBlobStorage", false);

            if (useBlobStorage)
            {
                services.AddTransient<IFileServices, BlobServices>();
            }
            else
            {
                services.AddTransient<IFileServices, FileService>();
            }
        }
    }
}
