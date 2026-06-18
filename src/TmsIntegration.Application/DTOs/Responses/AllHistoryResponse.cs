namespace TmsIntegration.Application.DTOs.Responses;

public sealed record AllHistoryResponse
{
    public int TotalEvents { get; init; }
    public IReadOnlyList<OrderEventResponse> Events { get; init; } = [];
}
