using System.Globalization;
using TmsIntegration.Application.Commands.AutoEmitToBeReturn;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Application.Mappers;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Exceptions;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Application.Commands.ProcessTmsEvent;

public sealed class ProcessTmsEventCommandHandler
    : ICommandHandler<ProcessTmsEventCommand, WebhookAcceptedResponse>
{
    private static readonly TimeSpan PeruUtcOffset = TimeSpan.FromHours(-5);

    private readonly IOrderRepository _orderRepository;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ICommandDispatcher _dispatcher;

    public ProcessTmsEventCommandHandler(
        IOrderRepository orderRepository,
        IOrderEventRepository orderEventRepository,
        ICommandDispatcher dispatcher)
    {
        _orderRepository = orderRepository;
        _orderEventRepository = orderEventRepository;
        _dispatcher = dispatcher;
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

        var wasApplied = false;
        string? rejectionReason = null;

        try
        {
            var order = await _orderRepository.GetByOrderNumberAsync(
                ev.Details.OrderNumber,
                cancellationToken);

            if (order is null)
            {
                rejectionReason = $"Order '{ev.Details.OrderNumber}' was not found in the OMS.";
                throw new NotFoundException("Order", ev.Details.OrderNumber);
            }

            if (EventStatusMapper.IsFinalStatus(order.CurrentStatus))
            {
                rejectionReason = $"Order is already in final state '{order.CurrentStatus}'.";
                throw new OrderInFinalStateException(
                    order.OrderNumber,
                    order.CurrentStatus.ToString());
            }

            var newStatus = EventStatusMapper.ToEventStatus(ev.Status);

            if (EventStatusMapper.IsVisitableStatus(newStatus))
                order.VisitCount++;

            order.CurrentStatus = newStatus;
            order.UpdatedAt = eventDatePeru;

            await _orderRepository.UpdateAsync(order, cancellationToken);

            wasApplied = true;

            if (order.VisitCount >= 3)
                return await _dispatcher.DispatchAsync<AutoEmitToBeReturnCommand, WebhookAcceptedResponse>(
                    new AutoEmitToBeReturnCommand
                    {
                        OrderNumber = order.OrderNumber,
                        EventDate = eventDatePeru
                    },
                    cancellationToken);

            return new WebhookAcceptedResponse
            {
                Message = "Event accepted and order status updated.",
                OrderNumber = order.OrderNumber,
                Status = ev.Status,
                VisitCount = order.VisitCount,
                AcceptedAt = DateTimeOffset.UtcNow
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (OrderInFinalStateException)
        {
            throw;
        }
        finally
        {
            await SafeRegisterHistoryAsync(
                ev.Details.OrderNumber,
                ev.ServiceType,
                ev.DispatchType,
                ev.Status,
                ev.SubStatus,
                ev.VehicleCode,
                ev.CourierName,
                eventDatePeru,
                wasApplied,
                rejectionReason,
                cancellationToken);
        }
    }

    private async Task SafeRegisterHistoryAsync(
        string orderNumber,
        string serviceType,
        string dispatchType,
        string status,
        string? subStatus,
        string vehicleCode,
        string courierName,
        DateTimeOffset eventDate,
        bool wasApplied,
        string? rejectionReason,
        CancellationToken cancellationToken)
    {
        try
        {
            await _orderEventRepository.AddAsync(new OrderEvent
            {
                OrderNumber = orderNumber,
                ServiceType = serviceType,
                DispatchType = dispatchType,
                Status = status,
                SubStatus = subStatus,
                VehicleCode = vehicleCode,
                CourierName = courierName,
                EventDate = eventDate,
                RegisteredAt = DateTimeOffset.UtcNow,
                WasApplied = wasApplied,
                RejectionReason = rejectionReason
            }, cancellationToken);
        }
        catch
        {
            // El registro del historial no debe afectar el flujo principal.
        }
    }
}
