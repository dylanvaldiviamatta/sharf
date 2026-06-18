using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Domain.Entities;
using TmsIntegration.Domain.Enums;
using TmsIntegration.Domain.Exceptions;
using TmsIntegration.Domain.Interfaces.Repositories;
using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Application.Commands.AutoEmitToBeReturn;

public sealed class AutoEmitToBeReturnCommandHandler
    : ICommandHandler<AutoEmitToBeReturnCommand, WebhookAcceptedResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly INotificationStrategyFactory _notificationFactory;

    public AutoEmitToBeReturnCommandHandler(
        IOrderRepository orderRepository,
        IOrderEventRepository orderEventRepository,
        INotificationStrategyFactory notificationFactory)
    {
        _orderRepository = orderRepository;
        _orderEventRepository = orderEventRepository;
        _notificationFactory = notificationFactory;
    }

    public async Task<WebhookAcceptedResponse> HandleAsync(
        AutoEmitToBeReturnCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(
            command.OrderNumber, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", command.OrderNumber);

        order.CurrentStatus = EventStatus.ToBeReturn;
        order.UpdatedAt = command.EventDate;

        await _orderRepository.UpdateAsync(order, cancellationToken);

        await _orderEventRepository.AddAsync(new OrderEvent
        {
            OrderNumber = order.OrderNumber,
            ServiceType = "SYSTEM",
            DispatchType = "REVERSE",
            Status = "TO_BE_RETURN",
            EventDate = command.EventDate,
            RegisteredAt = DateTimeOffset.UtcNow,
            WasApplied = true
        }, cancellationToken);

        var strategy = _notificationFactory.Resolve(order.NotificationChannel);
        await strategy.NotifyAsync(order, "TO_BE_RETURN", cancellationToken);

        return new WebhookAcceptedResponse
        {
            Message = "Visit limit reached. Order automatically assigned to return.",
            OrderNumber = order.OrderNumber,
            Status = "TO_BE_RETURN",
            VisitCount = order.VisitCount,
            NotificationChannel = order.NotificationChannel.ToString(),
            AcceptedAt = DateTimeOffset.UtcNow
        };
    }
}
