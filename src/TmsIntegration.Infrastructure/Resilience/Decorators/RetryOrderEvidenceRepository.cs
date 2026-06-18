using Polly;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Infrastructure.Resilience;

namespace TmsIntegration.Infrastructure.Resilience.Decorators;

public sealed class RetryOrderEvidenceRepository : IOrderEvidenceRepository
{
    private readonly IOrderEvidenceRepository _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryOrderEvidenceRepository(IOrderEvidenceRepository inner)
    {
        _inner    = inner;
        _pipeline = RetryPolicies.Default;
    }

    public Task AddAsync(OrderEvidence evidence, CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask(_inner.AddAsync(evidence, ct)), cancellationToken)
            .AsTask();

    public Task<IReadOnlyList<OrderEvidence>> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask<IReadOnlyList<OrderEvidence>>(_inner.GetByOrderNumberAsync(orderNumber, ct)), cancellationToken)
            .AsTask();
}
