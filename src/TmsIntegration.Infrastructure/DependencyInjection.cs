using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Domain.Interfaces.Services;
using TmsIntegration.Infrastructure.Persistence.Repositories;
using TmsIntegration.Infrastructure.Resilience.Decorators;
using TmsIntegration.Infrastructure.Services;
using TmsIntegration.Infrastructure.Services.Notifications;

namespace TmsIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // ── Repositorios en memoria (implementación concreta + decorador retry) ──
        services.AddSingleton<InMemoryOrderRepository>();
        services.AddSingleton<IOrderRepository>(sp =>
            new RetryOrderRepository(sp.GetRequiredService<InMemoryOrderRepository>()));

        services.AddSingleton<InMemoryOrderEventRepository>();
        services.AddSingleton<IOrderEventRepository>(sp =>
            new RetryOrderEventRepository(sp.GetRequiredService<InMemoryOrderEventRepository>()));

        services.AddSingleton<InMemoryOrderEvidenceRepository>();
        services.AddSingleton<IOrderEvidenceRepository>(sp =>
            new RetryOrderEvidenceRepository(sp.GetRequiredService<InMemoryOrderEvidenceRepository>()));

        // ── Servicio de evidencias (implementación concreta + decorador retry) ──
        services.AddSingleton<MockEvidenceStorageService>();
        services.AddSingleton<IEvidenceStorageService>(sp =>
            new RetryEvidenceStorageService(sp.GetRequiredService<MockEvidenceStorageService>()));

        // ── Estrategias de notificación + factory (con decorador retry) ──────
        services.AddSingleton<INotificationStrategy, PushNotificationStrategy>();
        services.AddSingleton<INotificationStrategy, EmailNotificationStrategy>();
        services.AddSingleton<INotificationStrategy, SmsNotificationStrategy>();
        services.AddSingleton<NotificationStrategyFactory>();
        services.AddSingleton<INotificationStrategyFactory>(sp =>
            new RetryNotificationStrategyFactory(sp.GetRequiredService<NotificationStrategyFactory>()));

        return services;
    }
}
