namespace TmsIntegration.Application.DTOs.Responses;

/// <summary>
/// Respuesta devuelta cuando un evento individual es aceptado (HTTP 202).
/// </summary>
public sealed record WebhookAcceptedResponse
{
    public string Message { get; init; } = "Event accepted for processing.";
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int VisitCount { get; init; }
    public IReadOnlyList<string> StoredEvidences { get; init; } = [];
    public DateTimeOffset AcceptedAt { get; init; } = DateTimeOffset.UtcNow;
}
