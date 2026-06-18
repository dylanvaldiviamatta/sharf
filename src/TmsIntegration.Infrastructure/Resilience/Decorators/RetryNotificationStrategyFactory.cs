using Polly;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Interfaces.Services;
using TmsIntegration.Infrastructure.Resilience;

namespace TmsIntegration.Infrastructure.Resilience.Decorators;

/// <summary>
/// Decora INotificationStrategyFactory para que la estrategia resuelta
/// ejecute NotifyAsync con política de reintentos.
/// </summary>
public sealed class RetryNotificationStrategyFactory : INotificationStrategyFactory
{
    private readonly INotificationStrategyFactory _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryNotificationStrategyFactory(INotificationStrategyFactory inner)
    {
        _inner    = inner;
        _pipeline = RetryPolicies.Default;
    }

    public INotificationStrategy Resolve(NotificationChannel channel)
    {
        var strategy = _inner.Resolve(channel);
        return new RetryNotificationStrategyWrapper(strategy, _pipeline);
    }
}

/// <summary>
/// Envuelve una INotificationStrategy aplicando retry en NotifyAsync.
/// No necesita ser registrado en DI: es creado por RetryNotificationStrategyFactory.
/// </summary>
internal sealed class RetryNotificationStrategyWrapper : INotificationStrategy
{
    private readonly INotificationStrategy _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryNotificationStrategyWrapper(INotificationStrategy inner, ResiliencePipeline pipeline)
    {
        _inner    = inner;
        _pipeline = pipeline;
    }

    public NotificationChannel Channel => _inner.Channel;

    public Task NotifyAsync(Order order, string newStatus, CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask(_inner.NotifyAsync(order, newStatus, ct)), cancellationToken)
            .AsTask();
}
