using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Requests;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Application.Commands.ProcessTmsEventBatch;

/// <summary>
/// Encapsula una lista de eventos TMS para procesamiento en lote.
/// </summary>
public sealed record ProcessTmsEventBatchCommand(IReadOnlyList<TmsEventRequest> Events)
    : ICommand<BatchWebhookResponse>;
