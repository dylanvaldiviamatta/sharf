using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Domain.Interfaces.Repositories;

namespace TmsIntegration.Application.Queries.GetAllHistory;

public sealed class GetAllHistoryQueryHandler
    : ICommandHandler<GetAllHistoryQuery, AllHistoryResponse>
{
    private readonly IOrderEventRepository _orderEventRepository;

    public GetAllHistoryQueryHandler(IOrderEventRepository orderEventRepository)
    {
        _orderEventRepository = orderEventRepository;
    }

    public async Task<AllHistoryResponse> HandleAsync(
        GetAllHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var events = await _orderEventRepository.GetAllAsync(cancellationToken);

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

        return new AllHistoryResponse
        {
            TotalEvents = eventResponses.Count,
            Events = eventResponses
        };
    }
}
