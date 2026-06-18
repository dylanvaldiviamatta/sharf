namespace TmsIntegration.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un recurso solicitado no existe en el sistema.
/// Produce un HTTP 404 Not Found.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string resourceName, string key)
        : base($"'{resourceName}' with key '{key}' was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
