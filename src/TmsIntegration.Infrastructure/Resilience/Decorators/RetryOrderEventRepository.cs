using Polly;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Infrastructure.Resilience;

namespace TmsIntegration.Infrastructure.Resilience.Decorators;

public sealed class RetryOrderEventRepository : IOrderEventRepository
{
    private readonly IOrderEventRepository _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryOrderEventRepository(IOrderEventRepository inner)
    {
        _inner    = inner;
        _pipeline = RetryPolicies.Default;
    }

    public Task AddAsync(OrderEvent orderEvent, CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask(_inner.AddAsync(orderEvent, ct)), cancellationToken)
            .AsTask();

    public Task<IReadOnlyList<OrderEvent>> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask<IReadOnlyList<OrderEvent>>(_inner.GetByOrderNumberAsync(orderNumber, ct)), cancellationToken)
            .AsTask();

    public Task<IReadOnlyList<OrderEvent>> GetAllAsync(CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask<IReadOnlyList<OrderEvent>>(_inner.GetAllAsync(ct)), cancellationToken)
            .AsTask();
}
