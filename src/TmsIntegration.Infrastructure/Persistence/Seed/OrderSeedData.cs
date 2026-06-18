using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;

namespace TmsIntegration.Infrastructure.Persistence.Seed;

/// <summary>
/// Datos iniciales del OMS para el repositorio en memoria.
/// Simula el catálogo de pedidos que existirían en producción.
/// </summary>
public static class OrderSeedData
{
    private static readonly TimeSpan PeruOffset = TimeSpan.FromHours(-5);

    public static IReadOnlyList<Order> GetOrders() =>
    [
        new Order
        {
            OrderNumber    = "2500000006-01",
            ClientCode     = "01021755",
            ClientName     = "TIENDAS PERUANAS S.A.",
            CurrentStatus  = EventStatus.Planning,
            VisitCount     = 0,
            CreatedAt      = new DateTimeOffset(2025, 4, 1, 8, 0, 0, PeruOffset)
        },
        new Order
        {
            OrderNumber    = "2500000006-02",
            ClientCode     = "01021756",
            ClientName     = "SUPERMERCADOS PERUANOS S.A.",
            CurrentStatus  = EventStatus.Started,
            VisitCount     = 0,
            CreatedAt      = new DateTimeOffset(2025, 4, 1, 9, 0, 0, PeruOffset)
        },
        new Order
        {
            OrderNumber    = "2500000006-03",
            ClientCode     = "01021757",
            ClientName     = "FALABELLA PERU S.A.",
            CurrentStatus  = EventStatus.Collected,
            VisitCount     = 0,
            CreatedAt      = new DateTimeOffset(2025, 4, 1, 10, 0, 0, PeruOffset)
        },
        new Order
        {
            OrderNumber    = "2500000007-01",
            ClientCode     = "01021758",
            ClientName     = "RIPLEY PERU S.A.",
            CurrentStatus  = EventStatus.NotDelivered,
            VisitCount     = 1,
            CreatedAt      = new DateTimeOffset(2025, 4, 2, 8, 0, 0, PeruOffset)
        },
        new Order
        {
            OrderNumber    = "2500000007-02",
            ClientCode     = "01021759",
            ClientName     = "SAGA FALABELLA S.A.",
            CurrentStatus  = EventStatus.NotDelivered,
            VisitCount     = 2,
            CreatedAt      = new DateTimeOffset(2025, 4, 2, 9, 0, 0, PeruOffset)
        }
    ];
}
