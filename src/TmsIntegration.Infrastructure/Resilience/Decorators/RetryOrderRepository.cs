using Polly;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Infrastructure.Resilience;

namespace TmsIntegration.Infrastructure.Resilience.Decorators;

public sealed class RetryOrderRepository : IOrderRepository
{
    private readonly IOrderRepository _inner;
    private readonly ResiliencePipeline _pipeline;

    public RetryOrderRepository(IOrderRepository inner)
    {
        _inner    = inner;
        _pipeline = RetryPolicies.Default;
    }

    public Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask<Order?>(_inner.GetByOrderNumberAsync(orderNumber, ct)), cancellationToken)
            .AsTask();

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        => _pipeline
            .ExecuteAsync(ct => new ValueTask(_inner.UpdateAsync(order, ct)), cancellationToken)
            .AsTask();
}
