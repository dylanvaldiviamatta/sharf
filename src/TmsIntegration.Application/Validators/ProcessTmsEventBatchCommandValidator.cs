using TmsIntegration.Application.Commands.ProcessTmsEventBatch;
using TmsIntegration.Application.Common.Validation;

namespace TmsIntegration.Application.Validators;

/// <summary>
/// Valida el comando batch a nivel de colección.
/// </summary>
public sealed class ProcessTmsEventBatchCommandValidator : IValidator<ProcessTmsEventBatchCommand>
{
    private const int MaxBatchSize = 100;

    public ValidationResult Validate(ProcessTmsEventBatchCommand command)
    {
        var result = new ValidationResult();

        if (command.Events is null || command.Events.Count == 0)
        {
            result.AddError("events", "At least one event is required.");
            return result;
        }

        if (command.Events.Count > MaxBatchSize)
            result.AddError("events",
                $"Batch size exceeds the limit of {MaxBatchSize}. " +
                $"Received: {command.Events.Count} events.");

        return result;
    }
}
