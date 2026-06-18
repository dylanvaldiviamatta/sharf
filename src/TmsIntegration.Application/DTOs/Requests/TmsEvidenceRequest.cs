using System.Text.Json.Serialization;

namespace TmsIntegration.Application.DTOs.Requests;

/// <summary>
/// Evidencia digital (foto, firma, etc.).
/// </summary>
public sealed record TmsEvidenceRequest
{
    [JsonPropertyName("label")]
    public string Label { get; init; } = string.Empty;

    [JsonPropertyName("fileType")]
    public string FileType { get; init; } = string.Empty;

    [JsonPropertyName("fileName")]
    public string FileName { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}
