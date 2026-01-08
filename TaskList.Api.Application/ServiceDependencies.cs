using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskList.Api.Application.Interfaces.Services;
using TaskList.Api.Application.MappingProfiles;
using TaskList.Api.Application.Services.AuthenticationServices;
using TaskList.Api.Application.Services.UserServices;
using TaskList.Api.Domain.Users.Interfaces.Repositories;

namespace TaskList.Api.Application
{
    public static class ServiceDependencies
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRefreshUserService, AuthUserService>();
            services.AddTransient<ILoginAndRegisterUserService, AuthUserService>();
            services.AddTransient<IUserProfileService, UserProfileService>();
            services.AddAutoMapper(typeof(UserMappingProfiles));

            // Add application-related services here
            return services;
        }
    }
}
