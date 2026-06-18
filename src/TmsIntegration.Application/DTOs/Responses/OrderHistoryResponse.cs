namespace TmsIntegration.Application.DTOs.Responses;

public sealed record OrderHistoryResponse
{
    public string OrderNumber { get; init; } = string.Empty;
    public int TotalEvents { get; init; }
    public IReadOnlyList<OrderEventResponse> Events { get; init; } = [];
}
