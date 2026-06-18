using System.Globalization;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Domain.Exceptions;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Application.Commands.ProcessTmsEvent;

/// <summary>
/// Handler del procesamiento de un evento TMS individual.
/// </summary>
public sealed class ProcessTmsEventCommandHandler
    : ICommandHandler<ProcessTmsEventCommand, WebhookAcceptedResponse>
{
    private static readonly TimeSpan PeruUtcOffset = TimeSpan.FromHours(-5);

    private readonly IOrderRepository _orderRepository;

    public ProcessTmsEventCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<WebhookAcceptedResponse> HandleAsync(
        ProcessTmsEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var ev = command.Event;

        var localDateTime = DateTime.ParseExact(
            ev.EventDate,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);

        var eventDatePeru = new DateTimeOffset(localDateTime, PeruUtcOffset);

        var order = await _orderRepository.GetByOrderNumberAsync(
            ev.Details.OrderNumber,
            cancellationToken);

        if (order is null)
            throw new NotFoundException(
                "Order",
                ev.Details.OrderNumber);

        return new WebhookAcceptedResponse
        {
            Message = "Event accepted for processing.",
            OrderNumber = ev.Details.OrderNumber,
            Status = ev.Status,
            AcceptedAt = DateTimeOffset.UtcNow
        };
    }
}
