using TmsIntegration.Application.Common.Validation;

namespace TmsIntegration.Application.Exceptions;

/// <summary>
/// Se lanza cuando un comando falla la validación.
/// </summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
