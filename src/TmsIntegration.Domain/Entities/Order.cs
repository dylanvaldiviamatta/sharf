using TmsIntegration.Domain.Enums;

namespace TmsIntegration.Domain.Entities;

public class Order
{
    public string OrderNumber { get; init; } = string.Empty;
    public string ClientCode { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public EventStatus CurrentStatus { get; set; }
    public int VisitCount { get; set; }
    public NotificationChannel NotificationChannel { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
