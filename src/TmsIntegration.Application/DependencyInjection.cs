using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Application.Commands.AutoEmitToBeReturn;
using TmsIntegration.Application.Commands.ProcessTmsEvent;
using TmsIntegration.Application.Commands.ProcessTmsEventBatch;
using TmsIntegration.Application.Common.Dispatcher;
using TmsIntegration.Application.Common.Validation;
using TmsIntegration.Application.DTOs.Responses;
using TmsIntegration.Application.Queries.GetAllHistory;
using TmsIntegration.Application.Queries.GetOrderHistory;
using TmsIntegration.Application.Validators;

namespace TmsIntegration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Dispatcher (mediador de comandos)
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        // Handlers
        services.AddScoped<
            ICommandHandler<ProcessTmsEventCommand, WebhookAcceptedResponse>,
            ProcessTmsEventCommandHandler>();

        services.AddScoped<
            ICommandHandler<ProcessTmsEventBatchCommand, BatchWebhookResponse>,
            ProcessTmsEventBatchCommandHandler>();

        services.AddScoped<
            ICommandHandler<AutoEmitToBeReturnCommand, WebhookAcceptedResponse>,
            AutoEmitToBeReturnCommandHandler>();

        // Queries
        services.AddScoped<
            ICommandHandler<GetOrderHistoryQuery, OrderHistoryResponse>,
            GetOrderHistoryQueryHandler>();

        services.AddScoped<
            ICommandHandler<GetAllHistoryQuery, AllHistoryResponse>,
            GetAllHistoryQueryHandler>();

        // Validators
        services.AddScoped<
            IValidator<ProcessTmsEventCommand>,
            ProcessTmsEventCommandValidator>();

        services.AddScoped<
            IValidator<ProcessTmsEventBatchCommand>,
            ProcessTmsEventBatchCommandValidator>();

        return services;
    }
}
