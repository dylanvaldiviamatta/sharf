using TmsIntegration.Domain.Entities;

namespace TmsIntegration.Domain.Interfaces.Repositories;

/// <summary>
/// Contrato para acceder y modificar los pedidos del OMS.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste los cambios realizados sobre un pedido existente.
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}
