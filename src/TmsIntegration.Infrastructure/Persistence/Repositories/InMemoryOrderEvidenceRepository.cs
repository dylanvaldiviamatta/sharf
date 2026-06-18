using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Infrastructure.Persistence.Repositories;

public sealed class InMemoryOrderEvidenceRepository : IOrderEvidenceRepository
{
    private readonly List<OrderEvidence> _evidences = [];

    public Task AddAsync(OrderEvidence evidence, CancellationToken cancellationToken = default)
    {
        _evidences.Add(evidence);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<OrderEvidence>> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        var result = _evidences
            .Where(e => e.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<OrderEvidence>>(result);
    }
}
