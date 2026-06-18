using TmsIntegration.Domain.Entities;

namespace TmsIntegration.Domain.Interfaces.Repositories;

public interface IOrderEventRepository
{
    Task AddAsync(OrderEvent orderEvent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderEvent>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderEvent>> GetAllAsync(CancellationToken cancellationToken = default);
}
