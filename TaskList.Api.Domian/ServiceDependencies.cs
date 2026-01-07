using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskList.Api.Domian
{
    public static class ServiceDependencies
    {
        public static IServiceCollection AddDomainApplication(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

    }
}
