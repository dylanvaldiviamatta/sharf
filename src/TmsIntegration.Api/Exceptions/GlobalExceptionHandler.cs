using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TmsIntegration.Application.Exceptions;
using TmsIntegration.Domain.Exceptions;

namespace TmsIntegration.Api.Exceptions;

/// <summary>
/// Manejador global de excepciones no capturadas.
///
/// Mapeo de excepciones a HTTP:
/// - ValidationException -> 400 Bad Request  (errores de validación del comando)
/// - NotFoundException   -> 404 Not Found    (pedido no existe en el OMS)
/// - Exception           -> 500 Internal     (errores inesperados)
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception,
            "Unhandled exception [{ExceptionType}]: {Message}",
            exception.GetType().Name,
            exception.Message);

        var (statusCode, title, detail, errors) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = GetRfcLink(statusCode)
        };

        if (errors is not null)
            problemDetails.Extensions["errors"] = errors;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int statusCode, string title, string detail, object? errors)
        MapException(Exception exception) => exception switch
    {
        ValidationException validationEx => (
            StatusCodes.Status400BadRequest,
            "Validation Error",
            "One or more validation errors occurred.",
            validationEx.Errors
                .GroupBy(e => e.Property)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Message).ToArray())
        ),

        NotFoundException notFoundEx => (
            StatusCodes.Status404NotFound,
            "Not Found",
            notFoundEx.Message,
            null
        ),

        _ => (
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "An unexpected error occurred. Please try again later.",
            null
        )
    };

    private static string GetRfcLink(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        _   => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };
}
