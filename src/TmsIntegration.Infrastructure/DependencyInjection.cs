using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Domain.Interfaces.Services;
using TmsIntegration.Infrastructure.Persistence.Repositories;
using TmsIntegration.Infrastructure.Services;
using TmsIntegration.Infrastructure.Services.Notifications;

namespace TmsIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repositorios en memoria
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IOrderEventRepository, InMemoryOrderEventRepository>();
        services.AddSingleton<IOrderEvidenceRepository, InMemoryOrderEvidenceRepository>();

        // Servicio de almacenamiento de evidencias (mock)
        services.AddSingleton<IEvidenceStorageService, MockEvidenceStorageService>();

        // Estrategias de notificación (mock)
        services.AddSingleton<INotificationStrategy, PushNotificationStrategy>();
        services.AddSingleton<INotificationStrategy, EmailNotificationStrategy>();
        services.AddSingleton<INotificationStrategy, SmsNotificationStrategy>();
        services.AddSingleton<INotificationStrategyFactory, NotificationStrategyFactory>();

        return services;
    }
}

