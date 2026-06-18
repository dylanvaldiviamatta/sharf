using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Infrastructure.Persistence.Repositories;

public sealed class InMemoryOrderEventRepository : IOrderEventRepository
{
    private readonly List<OrderEvent> _events = [];

    public Task AddAsync(OrderEvent orderEvent, CancellationToken cancellationToken = default)
    {
        _events.Add(orderEvent);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<OrderEvent>> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        var result = _events
            .Where(e => e.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<OrderEvent>>(result);
    }

    public Task<IReadOnlyList<OrderEvent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = _events
            .OrderByDescending(e => e.RegisteredAt)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<OrderEvent>>(result);
    }
}
