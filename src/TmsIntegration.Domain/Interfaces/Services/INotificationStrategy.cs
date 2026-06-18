using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;

namespace TmsIntegration.Domain.Interfaces.Services;

public interface INotificationStrategy
{
    NotificationChannel Channel { get; }
    Task NotifyAsync(Order order, string newStatus, CancellationToken cancellationToken = default);
}
