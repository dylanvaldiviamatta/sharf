using Polly;
using Polly.Retry;

namespace TmsIntegration.Infrastructure.Resilience;

/// <summary>
/// Define la política de reintentos compartida por todos los decoradores.
///
/// Configuración:
/// - 3 reintentos (4 intentos en total)
/// - Backoff exponencial: 200 ms → 400 ms → 800 ms
/// - Jitter: evita que múltiples instancias reintenten al mismo tiempo
/// - Aplica a cualquier excepción (en producción se puede ajustar
///   para reintentar solo en excepciones transitorias específicas)
/// </summary>
public static class RetryPolicies
{
    public static readonly ResiliencePipeline Default = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType    = DelayBackoffType.Exponential,
            Delay          = TimeSpan.FromMilliseconds(200),
            UseJitter      = true
        })
        .Build();
}
