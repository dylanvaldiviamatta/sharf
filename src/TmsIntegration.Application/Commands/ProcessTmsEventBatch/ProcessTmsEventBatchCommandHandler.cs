using TmsIntegration.Application.Commands.ProcessTmsEvent;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Application.Exceptions;
using TmsIntegration.Domain.Exceptions;

namespace TmsIntegration.Application.Commands.ProcessTmsEventBatch;

/// <summary>
/// Handler del procesamiento en lote.
/// </summary>
public sealed class ProcessTmsEventBatchCommandHandler
    : ICommandHandler<ProcessTmsEventBatchCommand, BatchWebhookResponse>
{
    private readonly ICommandDispatcher _dispatcher;

    public ProcessTmsEventBatchCommandHandler(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task<BatchWebhookResponse> HandleAsync(
        ProcessTmsEventBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        var results = new List<BatchItemResult>(command.Events.Count);

        foreach (var ev in command.Events)
        {
            var itemResult = await ProcessSingleEventAsync(ev, cancellationToken);
            results.Add(itemResult);
        }

        return new BatchWebhookResponse
        {
            Processed = results.Count,
            Succeeded = results.Count(r => r.Success),
            Failed = results.Count(r => !r.Success),
            Results = results.AsReadOnly()
        };
    }


    private async Task<BatchItemResult> ProcessSingleEventAsync(
        DTOs.Requests.TmsEventRequest ev,
        CancellationToken cancellationToken)
    {
        try
        {
            var singleCommand = new ProcessTmsEventCommand(ev);
            var response = await _dispatcher
                .DispatchAsync<ProcessTmsEventCommand, WebhookAcceptedResponse>(
                    singleCommand,
                    cancellationToken);

            return new BatchItemResult
            {
                OrderNumber = ev.Details?.OrderNumber ?? string.Empty,
                Status = ev.Status,
                Success = true,
                HttpStatus = StatusCodes.Status202Accepted,
                Message = response.Message
            };
        }
        catch (ValidationException ex)
        {
            return new BatchItemResult
            {
                OrderNumber = ev.Details?.OrderNumber ?? "N/A",
                Status = ev.Status ?? "N/A",
                Success = false,
                HttpStatus = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Errors = ex.Errors
                    .Select(e => $"{e.Property}: {e.Message}")
                    .ToList()
                    .AsReadOnly()
            };
        }
        catch (NotFoundException ex)
        {
            return new BatchItemResult
            {
                OrderNumber = ev.Details?.OrderNumber ?? "N/A",
                Status = ev.Status ?? "N/A",
                Success = false,
                HttpStatus = StatusCodes.Status404NotFound,
                Message = ex.Message
            };
        }
    }
}

/// <summary>
/// Referencia a StatusCodes sin depender de un paquete web desde Application.
/// </summary>
file static class StatusCodes
{
    public const int Status202Accepted = 202;
    public const int Status400BadRequest = 400;
    public const int Status404NotFound = 404;
}
