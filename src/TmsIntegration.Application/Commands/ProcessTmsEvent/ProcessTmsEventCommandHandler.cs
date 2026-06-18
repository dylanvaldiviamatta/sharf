using System.Globalization;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Application.Mappers;
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
            throw new NotFoundException("Order", ev.Details.OrderNumber);

        if (EventStatusMapper.IsFinalStatus(order.CurrentStatus))
            throw new OrderInFinalStateException(
                order.OrderNumber,
                order.CurrentStatus.ToString());

        var newStatus = EventStatusMapper.ToEventStatus(ev.Status);

        if (EventStatusMapper.IsVisitableStatus(newStatus))
            order.VisitCount++;

        order.CurrentStatus = newStatus;
        order.UpdatedAt = eventDatePeru;

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new WebhookAcceptedResponse
        {
            Message = "Event accepted and order status updated.",
            OrderNumber = order.OrderNumber,
            Status = ev.Status,
            VisitCount = order.VisitCount,
            AcceptedAt = DateTimeOffset.UtcNow
        };
    }
}

