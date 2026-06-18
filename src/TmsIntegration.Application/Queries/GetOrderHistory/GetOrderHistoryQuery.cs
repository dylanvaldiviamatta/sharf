using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Application.Queries.GetOrderHistory;

public sealed record GetOrderHistoryQuery(string OrderNumber) : ICommand<OrderHistoryResponse>;
