using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Contracts.AbstractFactory;
using AlfaCharge.OcppServer.Factory;
using AlfaCharge.OcppServer.Helpers;
using AlfaCharge.OcppServer.Messages;
using AlfaCharge.OcppServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlfaCharge.OcppServer.WebSockets
{
    public sealed class OcppWebSocketServer
    {
        private readonly ILogger<OcppWebSocketServer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly OcppConnectionManager _connectionManager;
        private readonly JsonSerializerOptions _jsonOptions;

        public OcppWebSocketServer(
            ILogger<OcppWebSocketServer> logger,
            IServiceScopeFactory scopeFactory,
            OcppConnectionManager connectionManager,
            JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectionManager = connectionManager;
            _jsonOptions = jsonOptions;
        }

        public void MapOcppEndpoint(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // Accept both /ocpp and /ocpp/{chargePointId}
                if (!context.Request.Path.StartsWithSegments("/ocpp", out var remaining))
                {
                    await next();
                    return;
                }

                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("WebSocket request expected.");
                    return;
                }

                // Determine subprotocol
                var requested = context.WebSockets.WebSocketRequestedProtocols;
                string? chosenSubProtocol = null;

                // Prefer exact names per spec
                if (requested.Contains(OcppSubprotocols.Ocpp201))
                    chosenSubProtocol = OcppSubprotocols.Ocpp201;
                else if (requested.Contains(OcppSubprotocols.Ocpp16))
                    chosenSubProtocol = OcppSubprotocols.Ocpp16;

                if (chosenSubProtocol == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Unsupported WebSocket subprotocol. Expected 'ocpp1.6' or 'ocpp2.0.1'.");
                    return;
                }

                // Parse chargePointId from URL if present: /ocpp/{chargePointId}
                var chargePointId = ParseChargePointId(remaining);
                if (string.IsNullOrWhiteSpace(chargePointId))
                {
                    // Fallback: some chargers append uniqid after /ocpp without separator guarantees
                    // e.g., "/ocpp/CP123" or "/ocppCP123". Try last segment.
                    var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                    if (segments.Length >= 2)
                    {
                        chargePointId = segments.Last();
                    }
                    else
                    {
                        // As a last fallback, generate a temporary ID (not ideal for production)
                        chargePointId = $"unknown-{Guid.NewGuid():N}";
                    }
                }

                using var webSocket = await context.WebSockets.AcceptWebSocketAsync(chosenSubProtocol);
                await HandleConnectionAsync(context, webSocket, chosenSubProtocol, chargePointId);
            });
        }

        private static string? ParseChargePointId(PathString remaining)
        {
            // remaining: "/{chargePointId}" or empty
            if (remaining.HasValue)
            {
                var segs = remaining.Value!.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segs.Length >= 1)
                    return segs[0];
            }
            return null;
        }

        private async Task HandleConnectionAsync(HttpContext httpContext, WebSocket socket, string subProtocol, string chargePointId)
        {
            _logger.LogInformation("OCPP connection starting. CP={ChargePointId}, Subprotocol={Subprotocol}", chargePointId, subProtocol);

            if (!OcppSubprotocols.TryParse(subProtocol, out var version))
            {
                await socket.CloseAsync(WebSocketCloseStatus.ProtocolError, "Unsupported OCPP subprotocol", httpContext.RequestAborted);
                return;
            }

            // IMPORTANT: one DI scope per connection (fixes the lifetime bug)
            using var scope = _scopeFactory.CreateScope();
            var services = scope.ServiceProvider;


            // Resolve scoped services safely
            var logWriter = services.GetRequiredService<IOcppLogWriter>();           // Scoped OK
            var db = services.GetRequiredService<ApplicationDbContext>();

            // Create versioned handler factory using the *connection scope* provider
            IOcppHandlerFactory handlerFactory = version switch
            {
                OcppProtocolVersion.Ocpp16 => new Ocpp16HandlerFactory(services),
                OcppProtocolVersion.Ocpp201 => new Ocpp21HandlerFactory(services),
                _ => throw new NotSupportedException($"Protocol version {version} not supported.")
            };

            var connection = new OcppConnection(chargePointId, version, socket, _jsonOptions, httpContext.RequestAborted, logWriter);

            // Register connection
            if (!_connectionManager.TryAdd(connection))
            {
                _logger.LogWarning("ChargePointId {ChargePointId} already connected. Replacing connection.", chargePointId);
                _connectionManager.Remove(chargePointId);
                _connectionManager.TryAdd(connection);
            }

            var router = new OcppMessageRouter(handlerFactory, connection, _jsonOptions, _logger, services, logWriter);

            try
            {
                var buffer = new byte[16 * 1024]; // 16 KB segment buffer
                var sb = new StringBuilder();

                while (socket.State == WebSocketState.Open && !httpContext.RequestAborted.IsCancellationRequested)
                {
                    sb.Clear();
                    WebSocketReceiveResult result;

                    // Assemble fragmented frames
                    do
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), httpContext.RequestAborted);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            _logger.LogInformation("Close frame received from CP={ChargePointId}", chargePointId);
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", httpContext.RequestAborted);
                            // Exit the while loop gracefully
                            return;
                        }
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    var incomingJson = sb.ToString();
                    var responseJson = await router.RouteAsync(incomingJson);

                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        var bytes = Encoding.UTF8.GetBytes(responseJson);
                        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, httpContext.RequestAborted);
                    }
                }
            }
            catch (OperationCanceledException) { /* normal on shutdown/abort */ }
            catch (WebSocketException wse)
            {
                _logger.LogWarning(wse, "WebSocket error for CP={ChargePointId}", chargePointId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error for CP={ChargePointId}", chargePointId);
            }
            finally
            {
                _connectionManager.Remove(chargePointId);
                _logger.LogInformation("OCPP connection ended. CP={ChargePointId}", chargePointId);
            }
        }
    }
}