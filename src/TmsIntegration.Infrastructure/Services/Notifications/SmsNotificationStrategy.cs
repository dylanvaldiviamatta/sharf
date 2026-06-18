using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Infrastructure.Services.Notifications;

public sealed class SmsNotificationStrategy : INotificationStrategy
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public Task NotifyAsync(Order order, string newStatus, CancellationToken cancellationToken = default)
    {
        // Mock: simula el envío de un SMS al cliente.
        // En producción se integraría con Twilio, AWS SNS u otro proveedor de SMS.
        _ = $"[SMS] → Client: {order.ClientName} ({order.ClientCode}) | " +
            $"Order {order.OrderNumber}: {newStatus}";

        return Task.CompletedTask;
    }
}
