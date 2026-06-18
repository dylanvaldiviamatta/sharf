namespace TmsIntegration.Application.DTOs.Responses;

/// <summary>
/// Resultado individual dentro de una respuesta batch.
/// </summary>
public sealed record BatchItemResult
{
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool Success { get; init; }

    /// <summary>
    /// Código HTTP que correspondería si este evento se enviara individualmente.
    /// 202 = aceptado, 400 = inválido, 404 = pedido no encontrado.
    /// </summary>
    public int HttpStatus { get; init; }

    public string Message { get; init; } = string.Empty;

    /// <summary>Lista de errores; sólo presente cuando Success = false.</summary>
    public IReadOnlyList<string>? Errors { get; init; }
}

/// <summary>
/// Respuesta del endpoint batch con el resumen de todos los eventos procesados.
/// Siempre retorna HTTP 200 con un desglose por ítem.
/// </summary>
public sealed record BatchWebhookResponse
{
    public int Processed { get; init; }
    public int Succeeded { get; init; }
    public int Failed { get; init; }
    public IReadOnlyList<BatchItemResult> Results { get; init; } = [];
}
