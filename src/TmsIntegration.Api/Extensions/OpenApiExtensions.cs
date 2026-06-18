using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using Scalar.AspNetCore;

namespace TmsIntegration.Api.Extensions;

public static class OpenApiExtensions
{
    private const string ApiKeyScheme = "ApiKey";
    private const string ApiKeyValue  = "tms-secret-key-2026";

    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title       = "TmsIntegration API";
                document.Info.Version     = "v1";
                document.Info.Description =
                    "API de integración entre el TMS externo (Beetrack) y el OMS interno.\n\n" +
                    "Gestiona la recepción de eventos de transporte, actualización de estados, " +
                    "almacenamiento de evidencias, notificaciones al cliente y registro de historial.\n\n" +
                    "**Autenticación:** header `X-Api-Key` ya pre-configurado en todas las requests.\n\n" +
                    "**Pedidos disponibles en el seed:**\n\n" +
                    "| Pedido | Estado inicial | VisitCount | Canal |\n" +
                    "|---|---|---|---|\n" +
                    "| 2500000006-01 | Planning | 0 | Push |\n" +
                    "| 2500000006-02 | Started | 0 | Email |\n" +
                    "| 2500000006-03 | Collected | 0 | Sms |\n" +
                    "| 2500000007-01 | NotDelivered | 1 | Push |\n" +
                    "| 2500000007-02 | NotDelivered | 2 | Email |\n\n" +
                    "En el endpoint `POST /api/webhooks/tms/event` usa el selector " +
                    "de ejemplos para ver los Requisitos 4 (auto-devolución), 6 (evidencias) y 7 " +
                    "(notificación por canal) ya armados y listos para ejecutar.";

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    [ApiKeyScheme] = new OpenApiSecurityScheme
                    {
                        Type        = SecuritySchemeType.ApiKey,
                        In          = ParameterLocation.Header,
                        Name        = "X-Api-Key",
                        Description = $"API Key de autenticación. Valor: `{ApiKeyValue}`"
                    }
                };

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var path   = (context.Description.RelativePath ?? "").TrimStart('/');
                var method = (context.Description.HttpMethod   ?? "").ToUpperInvariant();

                if (operation.RequestBody?.Content is null ||
                    !operation.RequestBody.Content.TryGetValue("application/json", out var media))
                {
                    return Task.CompletedTask;
                }

                if (path == "api/webhooks/tms/event" && method == "POST")
                {
                    media.Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["1-simple"] = new OpenApiExample
                        {
                            Summary = "1. Actualización simple de estado",
                            Description =
                                "PLANNING -> STARTED sobre el pedido 2500000006-01. " +
                                "Revisa 'notificationChannel': Push (Req. 7).",
                            Value = JsonNode.Parse(Examples.SimpleStatusUpdate)
                        },
                        ["2-evidence"] = new OpenApiExample
                        {
                            Summary = "2. Requisito 6 — Evidencias almacenadas",
                            Description =
                                "DELIVERED con 2 evidencias sobre el pedido 2500000006-02. " +
                                "La respuesta incluye 'storedEvidences' con las URLs generadas " +
                                "por el mock de almacenamiento. Canal: Email (Req. 7).",
                            Value = JsonNode.Parse(Examples.DeliveredWithEvidence)
                        },
                        ["3-no-evidence"] = new OpenApiExample
                        {
                            Summary = "3. Requisito 6 — sin evidencias (NO_EVIDENCE)",
                            Description =
                                "DELIVERED con 'evidences: []' sobre el pedido 2500000007-01. " +
                                "Internamente se registra una entrada NO_EVIDENCE; la respuesta " +
                                "retorna 'storedEvidences: []'.",
                            Value = JsonNode.Parse(Examples.DeliveredNoEvidence)
                        },
                        ["4-auto-return"] = new OpenApiExample
                        {
                            Summary = "4. Requisito 4 — Auto-emisión de TO_BE_RETURN",
                            Description =
                                "NOT_DELIVERED sobre el pedido 2500000007-02 (VisitCount=2 en el seed). " +
                                "Este evento sube el contador a 3 y el sistema auto-emite TO_BE_RETURN. " +
                                "La respuesta muestra 'status: TO_BE_RETURN' y 'visitCount: 3'. " +
                                "Canal: Email (Req. 7). Nota: solo funciona si VisitCount sigue en 2 ",
                            Value = JsonNode.Parse(Examples.ThirdAttemptTriggersReturn)
                        },
                        ["5-sms-channel"] = new OpenApiExample
                        {
                            Summary = "5. Requisito 7 — Notificación por SMS",
                            Description =
                                "COLLECTED con evidencias sobre el pedido 2500000006-03. " +
                                "Su canal configurado en el seed es Sms; la respuesta debe " +
                                "mostrar 'notificationChannel: Sms'.",
                            Value = JsonNode.Parse(Examples.CollectedSmsChannel)
                        }
                    };
                }
                else if (path == "api/webhooks/tms/events" && method == "POST")
                {
                    media.Example = JsonNode.Parse(Examples.BatchEvents);
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("TmsIntegration API")
                .WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .AddPreferredSecuritySchemes(ApiKeyScheme)
                .AddApiKeyAuthentication(ApiKeyScheme, apiKey =>
                {
                    apiKey.Value = ApiKeyValue;
                });
        });

        return app;
    }

    private static class Examples
    {
        public const string SimpleStatusUpdate = """
            {
              "serviceType": "LAST_MILE",
              "dispatchType": "HOME_DELIVERY",
              "status": "STARTED",
              "subStatus": "EN RUTA",
              "vehicleCode": "LIMURB06VAN",
              "courierName": "Conductor 46",
              "details": {
                "orderNumber": "2500000006-01",
                "trackingNumber": "OE2500000006-01",
                "clientCode": "01021755",
                "clientName": "TIENDAS PERUANAS S.A.",
                "receivedBy": null,
                "comments": "Courier inició la ruta de entrega",
                "evidences": []
              },
              "eventDate": "2025-04-15 08:30:00"
            }
            """;

        public const string DeliveredWithEvidence = """
            {
              "serviceType": "LAST_MILE",
              "dispatchType": "HOME_DELIVERY",
              "status": "DELIVERED",
              "subStatus": "CLIENT RECEIVED",
              "vehicleCode": "LIMURB06VAN",
              "courierName": "Conductor 46",
              "details": {
                "orderNumber": "2500000006-02",
                "trackingNumber": "OE2500000006-02",
                "clientCode": "01021756",
                "clientName": "SUPERMERCADOS PERUANOS S.A.",
                "receivedBy": "Pepito Perez",
                "comments": "Entregado en domicilio",
                "evidences": [
                  {
                    "label": "Paquete",
                    "fileType": ".jpg",
                    "fileName": "delivered_201054.jpg",
                    "url": "https://beetrack.com/img/delivered_201054.jpg"
                  },
                  {
                    "label": "Fachada",
                    "fileType": ".jpg",
                    "fileName": "delivered_201055.jpg",
                    "url": "https://beetrack.com/img/delivered_201055.jpg"
                  }
                ]
              },
              "eventDate": "2025-04-15 16:53:07"
            }
            """;

        public const string DeliveredNoEvidence = """
            {
              "serviceType": "LAST_MILE",
              "dispatchType": "HOME_DELIVERY",
              "status": "DELIVERED",
              "subStatus": "CLIENT RECEIVED",
              "vehicleCode": "LIMURB07VAN",
              "courierName": "Conductor 12",
              "details": {
                "orderNumber": "2500000007-01",
                "trackingNumber": "OE2500000007-01",
                "clientCode": "01021758",
                "clientName": "RIPLEY PERU S.A.",
                "receivedBy": "Ana García",
                "comments": "Entregado sin foto por batería baja del dispositivo",
                "evidences": []
              },
              "eventDate": "2025-04-15 17:10:00"
            }
            """;

        public const string ThirdAttemptTriggersReturn = """
            {
              "serviceType": "LAST_MILE",
              "dispatchType": "HOME_DELIVERY",
              "status": "NOT_DELIVERED",
              "subStatus": "AUSENTE",
              "vehicleCode": "LIMURB06VAN",
              "courierName": "Conductor 46",
              "details": {
                "orderNumber": "2500000007-02",
                "trackingNumber": "OE2500000007-02",
                "clientCode": "01021759",
                "clientName": "SAGA FALABELLA S.A.",
                "receivedBy": null,
                "comments": "Tercer intento fallido: nadie en el domicilio",
                "evidences": []
              },
              "eventDate": "2025-04-15 18:00:00"
            }
            """;

        public const string CollectedSmsChannel = """
            {
              "serviceType": "LAST_MILE",
              "dispatchType": "HOME_DELIVERY",
              "status": "COLLECTED",
              "subStatus": "PAQUETE RECOGIDO",
              "vehicleCode": "LIMURB08VAN",
              "courierName": "Conductor 23",
              "details": {
                "orderNumber": "2500000006-03",
                "trackingNumber": "OE2500000006-03",
                "clientCode": "01021757",
                "clientName": "FALABELLA PERU S.A.",
                "receivedBy": null,
                "comments": "Paquete recogido en punto de origen",
                "evidences": [
                  {
                    "label": "Paquete",
                    "fileType": ".jpg",
                    "fileName": "collected_301002.jpg",
                    "url": "https://beetrack.com/img/collected_301002.jpg"
                  },
                  {
                    "label": "Etiqueta",
                    "fileType": ".jpg",
                    "fileName": "collected_301003.jpg",
                    "url": "https://beetrack.com/img/collected_301003.jpg"
                  }
                ]
              },
              "eventDate": "2025-04-15 09:15:00"
            }
            """;

        public const string BatchEvents = """
            [
              {
                "serviceType": "LAST_MILE",
                "dispatchType": "HOME_DELIVERY",
                "status": "PLANNING",
                "subStatus": "ASIGNADO A RUTA",
                "vehicleCode": "LIMURB06VAN",
                "courierName": "Conductor 46",
                "details": {
                  "orderNumber": "2500000006-01",
                  "trackingNumber": "OE2500000006-01",
                  "clientCode": "01021755",
                  "clientName": "TIENDAS PERUANAS S.A.",
                  "receivedBy": null,
                  "comments": "Pedido asignado a ruta de entrega",
                  "evidences": []
                },
                "eventDate": "2025-04-15 08:00:00"
              },
              {
                "serviceType": "LAST_MILE",
                "dispatchType": "HOME_DELIVERY",
                "status": "STARTED",
                "subStatus": "EN RUTA",
                "vehicleCode": "LIMURB07VAN",
                "courierName": "Conductor 12",
                "details": {
                  "orderNumber": "2500000006-02",
                  "trackingNumber": "OE2500000006-02",
                  "clientCode": "01021756",
                  "clientName": "SUPERMERCADOS PERUANOS S.A.",
                  "receivedBy": null,
                  "comments": "Courier inició ruta",
                  "evidences": []
                },
                "eventDate": "2025-04-15 09:00:00"
              },
              {
                "serviceType": "LAST_MILE",
                "dispatchType": "HOME_DELIVERY",
                "status": "NOT_DELIVERED",
                "subStatus": "AUSENTE",
                "vehicleCode": "LIMURB06VAN",
                "courierName": "Conductor 46",
                "details": {
                  "orderNumber": "2500000006-03",
                  "trackingNumber": "OE2500000006-03",
                  "clientCode": "01021757",
                  "clientName": "FALABELLA PERU S.A.",
                  "receivedBy": null,
                  "comments": "No se encontró a nadie en el domicilio",
                  "evidences": []
                },
                "eventDate": "2025-04-15 14:00:00"
              }
            ]
            """;
    }
}
