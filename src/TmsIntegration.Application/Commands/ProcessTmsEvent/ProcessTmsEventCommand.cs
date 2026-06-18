using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Requests;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Application.Commands.ProcessTmsEvent;

/// <summary>
/// Encapsula el evento TMS recibido para ser procesado por su handler.
/// </summary>
public sealed record ProcessTmsEventCommand(TmsEventRequest Event)
    : ICommand<WebhookAcceptedResponse>;
