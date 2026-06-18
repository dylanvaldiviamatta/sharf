using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Application.Queries.GetOrderHistory;

public sealed class GetOrderHistoryQueryHandler
    : ICommandHandler<GetOrderHistoryQuery, OrderHistoryResponse>
{
    private readonly IOrderEventRepository _orderEventRepository;

    public GetOrderHistoryQueryHandler(IOrderEventRepository orderEventRepository)
    {
        _orderEventRepository = orderEventRepository;
    }

    public async Task<OrderHistoryResponse> HandleAsync(
        GetOrderHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var events = await _orderEventRepository.GetByOrderNumberAsync(
            query.OrderNumber,
            cancellationToken);

        var eventResponses = events
            .Select(e => new OrderEventResponse
            {
                Id = e.Id,
                OrderNumber = e.OrderNumber,
                ServiceType = e.ServiceType,
                DispatchType = e.DispatchType,
                Status = e.Status,
                SubStatus = e.SubStatus,
                VehicleCode = e.VehicleCode,
                CourierName = e.CourierName,
                EventDate = e.EventDate,
                RegisteredAt = e.RegisteredAt,
                WasApplied = e.WasApplied,
                RejectionReason = e.RejectionReason
            })
            .ToList()
            .AsReadOnly();

        return new OrderHistoryResponse
        {
            OrderNumber = query.OrderNumber,
            TotalEvents = eventResponses.Count,
            Events = eventResponses
        };
    }
}
