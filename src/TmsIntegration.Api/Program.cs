using TmsIntegration.Api.Endpoints;
using TmsIntegration.Api.Exceptions;
using TmsIntegration.Api.Middleware;
using TmsIntegration.Application;
using TmsIntegration.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Servicios 
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Manejo global de excepciones
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Pipeline
var app = builder.Build();

// Autenticación por API Key
app.UseMiddleware<ApiKeyMiddleware>();

// Convierte excepciones no manejadas en respuestas
app.UseExceptionHandler();

// Endpoints del webhook
app.MapWebhookEndpoints();

// Endpoints del historial
app.MapHistoryEndpoints();

app.Run();
