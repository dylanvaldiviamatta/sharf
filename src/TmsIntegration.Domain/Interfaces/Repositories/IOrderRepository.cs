using TmsIntegration.Domain.Entities;

namespace TmsIntegration.Domain.Interfaces.Repositories;

/// <summary>
/// Contrato para acceder a los pedidos del OMS.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
