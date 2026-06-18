using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.DTOs.Responses;

namespace TmsIntegration.Application.Queries.GetAllHistory;

public sealed record GetAllHistoryQuery : ICommand<AllHistoryResponse>;
