namespace TmsIntegration.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta actualizar un pedido que ya se encuentra
/// en un estado final (DELIVERED o RETURNED).
/// </summary>
public sealed class OrderInFinalStateException : Exception
{
    public string OrderNumber { get; }
    public string CurrentStatus { get; }

    public OrderInFinalStateException(string orderNumber, string currentStatus)
        : base($"Order '{orderNumber}' is already in final state '{currentStatus}' and cannot be updated.")
    {
        OrderNumber = orderNumber;
        CurrentStatus = currentStatus;
    }
}
