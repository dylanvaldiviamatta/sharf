using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Application.Queries.GetAllHistory;
using TmsIntegration.Application.Queries.GetOrderHistory;

namespace TmsIntegration.Api.Endpoints;

public static class HistoryEndpoints
{
    public static IEndpointRouteBuilder MapHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Order History");

        // Ruta literal primero para evitar colisión con {orderNumber}
        group.MapGet("/history", HandleGetAllHistoryAsync)
            .WithName("GetAllHistory")
            .WithSummary("Returns the full event history across all orders")
            .WithDescription(
                "Returns all registered events ordered by RegisteredAt descending. " +
                "Includes both applied (WasApplied=true) and rejected (WasApplied=false) events.")
            .Produces<AllHistoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{orderNumber}/history", HandleGetOrderHistoryAsync)
            .WithName("GetOrderHistory")
            .WithSummary("Returns the event history of a specific order")
            .WithDescription(
                "Returns all events registered for the given order number, " +
                "including rejected ones (WasApplied=false).")
            .Produces<OrderHistoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleGetAllHistoryAsync(
        ICommandDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var response = await dispatcher
            .DispatchAsync<GetAllHistoryQuery, AllHistoryResponse>(
                new GetAllHistoryQuery(), cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleGetOrderHistoryAsync(
        string orderNumber,
        ICommandDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var response = await dispatcher
            .DispatchAsync<GetOrderHistoryQuery, OrderHistoryResponse>(
                new GetOrderHistoryQuery(orderNumber), cancellationToken);

        return Results.Ok(response);
    }
}
