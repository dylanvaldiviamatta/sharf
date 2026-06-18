namespace TmsIntegration.Application.Common.Dispatcher;

/// <summary>
/// Contrato para los handlers que procesan un comando específico.
/// </summary>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
