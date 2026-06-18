using System.Text.Json.Serialization;

namespace TmsIntegration.Application.DTOs.Requests;

/// <summary>
/// Datos específicos del pedido contenidos dentro del evento TMS.
/// </summary>
public sealed record TmsEventDetailsRequest
{
    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; init; } = string.Empty;

    [JsonPropertyName("trackingNumber")]
    public string TrackingNumber { get; init; } = string.Empty;

    [JsonPropertyName("clientCode")]
    public string ClientCode { get; init; } = string.Empty;

    [JsonPropertyName("clientName")]
    public string ClientName { get; init; } = string.Empty;

    [JsonPropertyName("receivedBy")]
    public string? ReceivedBy { get; init; }

    [JsonPropertyName("comments")]
    public string? Comments { get; init; }

    [JsonPropertyName("evidences")]
    public IReadOnlyList<TmsEvidenceRequest> Evidences { get; init; } = [];
}
