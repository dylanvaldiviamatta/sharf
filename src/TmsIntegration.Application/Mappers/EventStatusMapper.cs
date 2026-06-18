using TmsIntegration.Domain.Enums;

namespace TmsIntegration.Application.Mappers;

/// <summary>
/// Convierte los valores de estado recibidos desde el TMS (strings)
/// al enum <see cref="EventStatus"/> del dominio.
/// </summary>
public static class EventStatusMapper
{
    private static readonly Dictionary<string, EventStatus> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["PLANNING"]        = EventStatus.Planning,
            ["STARTED"]         = EventStatus.Started,
            ["AT_PICKUP_POINT"] = EventStatus.AtPickupPoint,
            ["COLLECTED"]       = EventStatus.Collected,
            ["NOT_COLLECTED"]   = EventStatus.NotCollected,
            ["DELIVERED"]       = EventStatus.Delivered,
            ["NOT_DELIVERED"]   = EventStatus.NotDelivered,
            ["TO_BE_RETURN"]    = EventStatus.ToBeReturn,
            ["RETURNED"]        = EventStatus.Returned,
            ["NOT_RETURNED"]    = EventStatus.NotReturned
        };

    /// <summary>
    /// Convierte el status string del TMS al <see cref="EventStatus"/> correspondiente.
    /// </summary>
    public static EventStatus ToEventStatus(string tmsStatus)
    {
        if (Map.TryGetValue(tmsStatus, out var status))
            return status;

        throw new InvalidOperationException(
            $"Cannot map TMS status '{tmsStatus}' to a known EventStatus. " +
            "Ensure the validator rejects unknown statuses before reaching this point.");
    }

    /// <summary>
    /// Indica si un <see cref="EventStatus"/> es un estado final
    /// que impide actualizaciones posteriores.
    /// </summary>
    public static bool IsFinalStatus(EventStatus status) =>
        status is EventStatus.Delivered or EventStatus.Returned;
}
