using Microsoft.Extensions.DependencyInjection;
using TmsIntegration.Application.Common.Validation;
using TmsIntegration.Application.Exceptions;

namespace TmsIntegration.Application.Common.Dispatcher;

/// <summary>
/// Implementación del dispatcher de comandos.
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        if (validator is not null)
        {
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellationToken);
    }
}
