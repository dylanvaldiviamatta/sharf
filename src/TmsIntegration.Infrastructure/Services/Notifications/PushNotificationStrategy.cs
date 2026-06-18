using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Infrastructure.Services.Notifications;

public sealed class PushNotificationStrategy : INotificationStrategy
{
    public NotificationChannel Channel => NotificationChannel.Push;

    public Task NotifyAsync(Order order, string newStatus, CancellationToken cancellationToken = default)
    {
        // Mock: simula el envío de una notificación push al dispositivo del cliente.
        // En producción se integraría con FCM, APNs u otro proveedor de push.
        _ = $"[PUSH] → Client: {order.ClientName} ({order.ClientCode}) | " +
            $"Order: {order.OrderNumber} | Status: {newStatus}";

        return Task.CompletedTask;
    }
}
