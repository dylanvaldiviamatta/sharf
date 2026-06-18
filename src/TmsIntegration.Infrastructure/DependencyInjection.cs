using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Infrastructure.Persistence.Repositories;

namespace TmsIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        return services;
    }
}
