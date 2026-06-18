namespace TmsIntegration.Domain.Entities;

public sealed class OrderEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string OrderNumber { get; init; } = string.Empty;
    public string ServiceType { get; init; } = string.Empty;
    public string DispatchType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? SubStatus { get; init; }
    public string VehicleCode { get; init; } = string.Empty;
    public string CourierName { get; init; } = string.Empty;
    public DateTimeOffset EventDate { get; init; }
    public DateTimeOffset RegisteredAt { get; init; } = DateTimeOffset.UtcNow;
    public bool WasApplied { get; init; }
    public string? RejectionReason { get; init; }
}
