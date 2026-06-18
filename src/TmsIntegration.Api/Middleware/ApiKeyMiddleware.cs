using System.Net.Mime;
using System.Text.Json;

namespace TmsIntegration.Api.Middleware;

/// <summary>
/// Middleware que valida la API Key en el header "X-Api-Key" antes de permitir el acceso a cualquier endpoint.
/// </summary>
public sealed class ApiKeyMiddleware
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyConfigSection = "ApiKey";

    private readonly RequestDelegate _next;
    private readonly string _configuredApiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;

        _configuredApiKey = configuration[ApiKeyConfigSection]
            ?? throw new InvalidOperationException(
                $"Missing required configuration: '{ApiKeyConfigSection}'. " +
                "Add it to appsettings.json.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var receivedKey)
            || string.IsNullOrWhiteSpace(receivedKey))
        {
            await WriteUnauthorizedAsync(context, $"Missing '{ApiKeyHeaderName}' header.");
            return;
        }

        if (!CryptographicEquals(_configuredApiKey, receivedKey.ToString()))
        {
            await WriteUnauthorizedAsync(context, "Invalid API Key.");
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string detail)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            title = "Unauthorized",
            status = 401,
            detail
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    /// <summary>
    /// Comparación de strings en tiempo constante.
    /// </summary>
    private static bool CryptographicEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];

        return result == 0;
    }
}
