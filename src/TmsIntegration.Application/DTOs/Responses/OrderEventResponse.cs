namespace TmsIntegration.Application.DTOs.Responses;

public sealed record OrderEventResponse
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string ServiceType { get; init; } = string.Empty;
    public string DispatchType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? SubStatus { get; init; }
    public string VehicleCode { get; init; } = string.Empty;
    public string CourierName { get; init; } = string.Empty;
    public DateTimeOffset EventDate { get; init; }
    public DateTimeOffset RegisteredAt { get; init; }
    public bool WasApplied { get; init; }
    public string? RejectionReason { get; init; }
}
