namespace TmsIntegration.Application.Common.Validation;

/// <summary>
/// Contenedor del resultado de validación.
/// </summary>
public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors = [];

    public bool IsValid => _errors.Count == 0;
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();
    public ValidationResult AddError(string property, string message)
    {
        _errors.Add(new ValidationError(property, message));
        return this;
    }
}
