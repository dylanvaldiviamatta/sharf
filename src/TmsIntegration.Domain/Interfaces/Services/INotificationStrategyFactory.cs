using TmsIntegration.Domain.Enums;

namespace TmsIntegration.Domain.Interfaces.Services;

public interface INotificationStrategyFactory
{
    INotificationStrategy Resolve(NotificationChannel channel);
}
