using TmsIntegration.Domain.Entities;

namespace TmsIntegration.Domain.Interfaces.Repositories;

public interface IOrderEvidenceRepository
{
    Task AddAsync(OrderEvidence evidence, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderEvidence>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
