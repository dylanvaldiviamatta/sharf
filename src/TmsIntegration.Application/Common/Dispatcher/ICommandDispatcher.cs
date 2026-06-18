namespace TmsIntegration.Application.Common.Dispatcher;

/// <summary>
/// Despachador central de comandos.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}
