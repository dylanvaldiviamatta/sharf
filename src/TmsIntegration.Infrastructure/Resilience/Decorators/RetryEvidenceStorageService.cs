using Polly;
using TmsIntegration.Domain.Interfaces.Services;
using TmsIntegration.Infrastructure.Resilience;

namespace TmsIntegration.Infrastructure.Resilience.Decorators;

public sealed class RetryEvidenceStorageService : IEvidenceStorageService
{
    private readonly IEvidenceStorageService _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryEvidenceStorageService(IEvidenceStorageService inner)
    {
        _inner    = inner;
        _pipeline = RetryPolicies.Default;
    }

    public Task<string> StoreAsync(
        string orderNumber,
        string fileName,
        string sourceUrl,
        CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask<string>(_inner.StoreAsync(orderNumber, fileName, sourceUrl, ct)), cancellationToken)
            .AsTask();
}
