using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Infrastructure.Persistence.Seed;

namespace TmsIntegration.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de pedidos usando un diccionario en memoria.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<string, Order> _store;

    public InMemoryOrderRepository()
    {
        _store = OrderSeedData.GetOrders()
            .ToDictionary(o => o.OrderNumber, StringComparer.OrdinalIgnoreCase);
    }

    public Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(orderNumber, out var order);
        return Task.FromResult(order);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _store[order.OrderNumber] = order;
        return Task.CompletedTask;
    }
}
