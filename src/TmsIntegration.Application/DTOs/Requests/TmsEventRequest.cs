using System.Text.Json.Serialization;

namespace TmsIntegration.Application.DTOs.Requests;

/// <summary>
/// Trama principal del evento emitido por el TMS.
/// </summary>
public sealed record TmsEventRequest
{
    [JsonPropertyName("serviceType")]
    public string ServiceType { get; init; } = string.Empty;

    [JsonPropertyName("dispatchType")]
    public string DispatchType { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("subStatus")]
    public string? SubStatus { get; init; }

    [JsonPropertyName("vehicleCode")]
    public string VehicleCode { get; init; } = string.Empty;

    [JsonPropertyName("courierName")]
    public string CourierName { get; init; } = string.Empty;

    [JsonPropertyName("details")]
    public TmsEventDetailsRequest Details { get; init; } = new();

    [JsonPropertyName("eventDate")]
    public string EventDate { get; init; } = string.Empty;
}
