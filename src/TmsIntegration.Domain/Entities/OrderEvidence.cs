namespace TmsIntegration.Domain.Entities;

public sealed class OrderEvidence
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string FileType { get; init; } = string.Empty;
    public string OriginalUrl { get; init; } = string.Empty;
    public string StoredUrl { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTimeOffset StoredAt { get; init; } = DateTimeOffset.UtcNow;
}
