namespace TmsIntegration.Application.Common.Validation;

/// <summary>
/// Contrato para validadores de comandos.
/// </summary>
public interface IValidator<T>
{
    ValidationResult Validate(T instance);
}
