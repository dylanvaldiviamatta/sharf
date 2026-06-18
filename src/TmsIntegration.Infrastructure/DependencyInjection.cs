using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Domain.Interfaces.Services;
using TmsIntegration.Infrastructure.Persistence.Repositories;
using TmsIntegration.Infrastructure.Services;

namespace TmsIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IOrderEventRepository, InMemoryOrderEventRepository>();
        services.AddSingleton<IOrderEvidenceRepository, InMemoryOrderEvidenceRepository>();
        services.AddSingleton<IEvidenceStorageService, MockEvidenceStorageService>();
        return services;
    }
}
