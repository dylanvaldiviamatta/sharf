using System.Globalization;
using TmsIntegration.Application.Commands.ProcessTmsEvent;
using TmsIntegration.Application.Common.Validation;

namespace TmsIntegration.Application.Validators;

/// <summary>
/// Valida el comando de procesamiento de un evento individual.
/// </summary>
public sealed class ProcessTmsEventCommandValidator : IValidator<ProcessTmsEventCommand>
{
    private static readonly HashSet<string> ValidServiceTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "LAST_MILE", "PICKUP", "RETURN"
    };

    private static readonly HashSet<string> ValidDispatchTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "HOME_DELIVERY", "STORE_WITHDRAWAL", "REVERSE"
    };

    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "PLANNING", "STARTED", "AT_PICKUP_POINT", "COLLECTED", "NOT_COLLECTED",
        "DELIVERED", "NOT_DELIVERED", "TO_BE_RETURN", "RETURNED", "NOT_RETURNED"
    };

    private const string EventDateFormat = "yyyy-MM-dd HH:mm:ss";

    public ValidationResult Validate(ProcessTmsEventCommand command)
    {
        var result = new ValidationResult();
        var ev = command.Event;

        if (ev is null)
        {
            result.AddError("event", "Event payload is required.");
            return result;
        }

        ValidateServiceType(ev.ServiceType, result);
        ValidateDispatchType(ev.DispatchType, result);
        ValidateStatus(ev.Status, result);
        ValidateVehicleCode(ev.VehicleCode, result);
        ValidateCourierName(ev.CourierName, result);
        ValidateEventDate(ev.EventDate, result);
        ValidateDetails(ev.Details, result);

        return result;
    }

    private static void ValidateServiceType(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError("serviceType", "ServiceType is required.");
            return;
        }

        if (!ValidServiceTypes.Contains(value))
            result.AddError("serviceType",
                $"ServiceType '{value}' is not valid. " +
                $"Accepted values: {string.Join(", ", ValidServiceTypes)}.");
    }

    private static void ValidateDispatchType(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError("dispatchType", "DispatchType is required.");
            return;
        }

        if (!ValidDispatchTypes.Contains(value))
            result.AddError("dispatchType",
                $"DispatchType '{value}' is not valid. " +
                $"Accepted values: {string.Join(", ", ValidDispatchTypes)}.");
    }

    private static void ValidateStatus(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError("status", "Status is required.");
            return;
        }

        if (!ValidStatuses.Contains(value))
            result.AddError("status",
                $"Status '{value}' is not valid. " +
                $"Accepted values: {string.Join(", ", ValidStatuses)}.");
    }

    private static void ValidateVehicleCode(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
            result.AddError("vehicleCode", "VehicleCode is required.");
    }

    private static void ValidateCourierName(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
            result.AddError("courierName", "CourierName is required.");
    }

    private static void ValidateEventDate(string value, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError("eventDate", "EventDate is required.");
            return;
        }

        if (!DateTime.TryParseExact(value, EventDateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            result.AddError("eventDate",
                $"EventDate '{value}' is not valid. " +
                $"Expected format: '{EventDateFormat}' (UTC-5 Peru time).");
        }
    }

    private static void ValidateDetails(
        DTOs.Requests.TmsEventDetailsRequest? details,
        ValidationResult result)
    {
        if (details is null)
        {
            result.AddError("details", "Details are required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(details.OrderNumber))
            result.AddError("details.orderNumber", "OrderNumber is required.");
        else if (details.OrderNumber.Length > 50)
            result.AddError("details.orderNumber", "OrderNumber must not exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(details.TrackingNumber))
            result.AddError("details.trackingNumber", "TrackingNumber is required.");
        else if (details.TrackingNumber.Length > 50)
            result.AddError("details.trackingNumber", "TrackingNumber must not exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(details.ClientCode))
            result.AddError("details.clientCode", "ClientCode is required.");
        else if (details.ClientCode.Length > 20)
            result.AddError("details.clientCode", "ClientCode must not exceed 20 characters.");

        if (string.IsNullOrWhiteSpace(details.ClientName))
            result.AddError("details.clientName", "ClientName is required.");
        else if (details.ClientName.Length > 200)
            result.AddError("details.clientName", "ClientName must not exceed 200 characters.");
    }
}
