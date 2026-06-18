namespace TmsIntegration.Application.Common.Validation;

/// <summary>
/// Representa un error de validación con la propiedad que falló y el motivo.
/// </summary>
public sealed record ValidationError(string Property, string Message);
