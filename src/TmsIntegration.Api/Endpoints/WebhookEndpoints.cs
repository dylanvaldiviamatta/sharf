using Microsoft.AspNetCore.Mvc;
using TmsIntegration.Application.Commands.ProcessTmsEvent;
using TmsIntegration.Application.Commands.ProcessTmsEventBatch;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Requests;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Api.Endpoints;

/// <summary>
/// Define los endpoints del webhook del TMS.
///
/// POST /api/webhooks/tms/event   -> Recibe un único evento TMS.
/// POST /api/webhooks/tms/events  -> Recibe un lote de eventos TMS.
///
/// Ambos requieren el header "X-Api-Key".
/// </summary>
public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/webhooks/tms")
            .WithTags("TMS Webhooks");

        group.MapPost("/event", HandleSingleEventAsync)
            .WithName("ProcessTmsEvent")
            .WithSummary("Receives and validates a single TMS event")
            .WithDescription(
                "Validates the event structure and verifies the order exists in the OMS. " +
                "Returns 202 Accepted if valid, 400 for validation errors, " +
                "404 if the order is not found, 401 if the API Key is missing or invalid.")
            .Produces<WebhookAcceptedResponse>(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/events", HandleBatchEventsAsync)
            .WithName("ProcessTmsEventBatch")
            .WithSummary("Receives and validates a batch of TMS events (max 100)")
            .WithDescription(
                "Processes each event independently. " +
                "A failed item does not stop processing of remaining items. " +
                "Always returns 200 with a per-item result summary.")
            .Produces<BatchWebhookResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleSingleEventAsync(
        [FromBody] TmsEventRequest request,
        ICommandDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new ProcessTmsEventCommand(request);
        var response = await dispatcher
            .DispatchAsync<ProcessTmsEventCommand, WebhookAcceptedResponse>(
                command, cancellationToken);

        return Results.Accepted(uri: null, value: response);
    }

    private static async Task<IResult> HandleBatchEventsAsync(
        [FromBody] List<TmsEventRequest> requests,
        ICommandDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new ProcessTmsEventBatchCommand(requests.AsReadOnly());
        var response = await dispatcher
            .DispatchAsync<ProcessTmsEventBatchCommand, BatchWebhookResponse>(
                command, cancellationToken);

        return Results.Ok(response);
    }
}
