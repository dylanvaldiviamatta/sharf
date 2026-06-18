using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Infrastructure.Services.Notifications;

public sealed class NotificationStrategyFactory : INotificationStrategyFactory
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotificationStrategy> _strategies;

    public NotificationStrategyFactory(IEnumerable<INotificationStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Channel);
    }

    public INotificationStrategy Resolve(NotificationChannel channel)
    {
        if (_strategies.TryGetValue(channel, out var strategy))
            return strategy;

        throw new InvalidOperationException(
            $"No notification strategy registered for channel '{channel}'.");
    }
}
