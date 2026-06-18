using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Application.Commands.AutoEmitToBeReturn;

public sealed record AutoEmitToBeReturnCommand : ICommand<WebhookAcceptedResponse>
{
    public string OrderNumber { get; init; } = string.Empty;
    public DateTimeOffset EventDate { get; init; }
}
