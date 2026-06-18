using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Infrastructure.Services.Notifications;

public sealed class EmailNotificationStrategy : INotificationStrategy
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public Task NotifyAsync(Order order, string newStatus, CancellationToken cancellationToken = default)
    {
        // Mock: simula el envío de un correo al cliente.
        // En producción se integraría con SendGrid, SES u otro proveedor de email.
        _ = $"[EMAIL] → To: {order.ClientName} ({order.ClientCode}) | " +
            $"Subject: Order {order.OrderNumber} updated | Status: {newStatus}";

        return Task.CompletedTask;
    }
}
