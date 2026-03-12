using System.Text.Json;
using AlfaCharge.Api.DTO;
using AlfaCharge.Api.Helpers;
using AlfaCharge.OcppServer.Helpers;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace AlfaCharge.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class ChargePointController : ControllerBase
    {
        private readonly OcppConnectionManager _connections;
        private readonly ILogger<ChargePointController> _logger;

        public ChargePointController(OcppConnectionManager connections, ILogger<ChargePointController> logger)
        {
            _connections = connections;
            _logger = logger;
        }

        // --------------------------------------------------------------------
        // 1) GET /api/cp  -> list of connected chargePointIds (and protocol)
        // --------------------------------------------------------------------

        /// <summary>
        /// Returns a list of currently connected charge points.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConnectedCpDto>), StatusCodes.Status200OK)]
        public IActionResult GetConnected()
        {
            var result = _connections.ConnectedIds
                .Select(id => _connections.TryGet(id, out var conn) && conn != null
                    ? new ConnectedCpDto { ChargePointId = id, Protocol = conn.ProtocolVersion.ToString() }
                    : new ConnectedCpDto { ChargePointId = id, Protocol = "Unknown" })
                .OrderBy(x => x.ChargePointId)
                .ToList();

            return Ok(result);
        }

        // --------------------------------------------------------------------
        // 2) POST /api/cp/{id}/ping  -> CSMS-initiated connectivity check
        //    Implemented using OCPP DataTransfer (vendor-neutral way).
        // --------------------------------------------------------------------

        /// <summary>
        /// Sends a DataTransfer "Ping" to the charger to verify the connection is alive.
        /// </summary>
        /// <remarks>
        /// OCPP 1.6 &amp; 2.0.1 DataTransfer is used for a generic, vendor-agnostic ping.
        /// Request body is optional. If omitted, defaults to vendorId "AlphaCharge" and messageId "Ping".
        /// </remarks>
        /// <param name="id">Charge point id.</param>
        /// <param name="request">
        /// Optional request:
        /// { "vendorId": "AlphaCharge", "messageId": "Ping", "data": "any string or JSON", "timeoutSeconds": 15 }
        /// </param>
        [HttpPost("{id}/ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> PingAsync(string id, [FromBody] PingRequestDto? request, CancellationToken ct)
        {
            if (!_connections.TryGet(id, out var conn) || conn is null)
                return NotFound(new { message = $"Charge point '{id}' is not connected." });

            var vendorId = string.IsNullOrWhiteSpace(request?.VendorId) ? "AlphaCharge" : request!.VendorId!;
            var messageId = string.IsNullOrWhiteSpace(request?.MessageId) ? "Ping" : request!.MessageId!;
            var timeout = TimeSpan.FromSeconds(request?.TimeoutSeconds > 0 ? request!.TimeoutSeconds : 15);

            try
            {
                // DataTransfer payload differs slightly, but both versions accept vendorId/messageId/data.
                // We'll pass 'data' as string; chargers that expect JSON may echo it as a string.
                var payload = new
                {
                    vendorId,
                    messageId,
                    data = request?.Data // could be string or JSON; serializer will handle it
                };

                var resultJson = await conn.SendCallAsync("DataTransfer", payload, timeout, ct);

                return Ok(new
                {
                    chargePointId = id,
                    protocol = conn.ProtocolVersion.ToString(),
                    action = "DataTransfer",
                    request = payload,
                    response = JsonDocument.Parse(resultJson).RootElement
                });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "Timed out waiting for charger response." });
            }
            catch (OcppCallErrorException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Charger returned CALLERROR.",
                    errorCode = ex.ErrorCode,
                    description = ex.Message,
                    details = ParseHelper.TryParseOrEcho(ex.DetailsJson)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ping failed for CP={CP}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // 3) POST /api/cp/{id}/changeAvailability
        //    OCPP 1.6: { connectorId: 0|N, type: "Operative"|"Inoperative" }
        //    OCPP 2.0.1: { operationalStatus: "Operative"|"Inoperative", evseId?: N, connectorId?: N }
        // --------------------------------------------------------------------

        /// <summary>
        /// Changes the availability of a charger (entire CP, a specific EVSE or connector).
        /// </summary>
        /// <param name="id">Charge point id.</param>
        /// <param name="request">
        /// OCPP 1.6:
        /// { "type": "Operative"|"Inoperative", "connectorId": 0 (all) or N, "timeoutSeconds": 30 }
        /// OCPP 2.0.1:
        /// { "operationalStatus": "Operative"|"Inoperative", "evseId": N?, "connectorId": N?, "timeoutSeconds": 30 }
        /// </param>
        [HttpPost("{id}/changeAvailability")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> ChangeAvailabilityAsync(string id, [FromBody] ChangeAvailabilityRequestDto request, CancellationToken ct)
        {
            if (!_connections.TryGet(id, out var conn) || conn is null)
                return NotFound(new { message = $"Charge point '{id}' is not connected." });

            var timeout = TimeSpan.FromSeconds(request.TimeoutSeconds <= 0 ? 30 : request.TimeoutSeconds);

            try
            {
                const string action = "ChangeAvailability";
                object payload;

                if (conn.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                {
                    var type = request.Type ?? request.OperationalStatus ?? "Operative";
                    if (!ParseHelper.IsAny(type, "Operative", "Inoperative"))
                    {
                        return BadRequest(new
                        {
                            message = "For OCPP 1.6, 'type' must be 'Operative' or 'Inoperative'.",
                            provided = request.Type ?? request.OperationalStatus
                        });
                    }

                    // connectorId: 0 means "entire charge point" per OCPP 1.6
                    var connectorId = request.ConnectorId ?? 0;

                    payload = new
                    {
                        connectorId,
                        type = ParseHelper.NormalizeEnum(type, "Operative", "Inoperative")
                    };
                }
                else
                {
                    // OCPP 2.0.1 expects 'operationalStatus' and optional target scoping (evseId, connectorId).
                    var status = request.OperationalStatus ?? request.Type ?? "Operative";
                    if (!ParseHelper.IsAny(status, "Operative", "Inoperative"))
                    {
                        return BadRequest(new
                        {
                            message = "For OCPP 2.0.1, 'operationalStatus' must be 'Operative' or 'Inoperative'.",
                            provided = request.OperationalStatus ?? request.Type
                        });
                    }

                    payload = new
                    {
                        operationalStatus = ParseHelper.NormalizeEnum(status, "Operative", "Inoperative"),
                        evseId = request.EvseId,
                        connectorId = request.ConnectorId
                    };
                }

                var resultJson = await conn.SendCallAsync(action, payload, timeout, ct);

                return Ok(new
                {
                    chargePointId = id,
                    protocol = conn.ProtocolVersion.ToString(),
                    action,
                    request = payload,
                    response = JsonDocument.Parse(resultJson).RootElement
                });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "Timed out waiting for charger response." });
            }
            catch (OcppCallErrorException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Charger returned CALLERROR.",
                    errorCode = ex.ErrorCode,
                    description = ex.Message,
                    details = ParseHelper.TryParseOrEcho(ex.DetailsJson)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangeAvailability failed for CP={CP}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // Existing Reset endpoint (kept for completeness)
        // --------------------------------------------------------------------

        /// <summary>
        /// Sends a Reset command to a connected charge point.
        /// </summary>
        [HttpPost("{id}/reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> ResetAsync(string id, [FromBody] ResetRequestDto request, CancellationToken ct)
        {
            if (!_connections.TryGet(id, out var conn) || conn is null)
                return NotFound(new { message = $"Charge point '{id}' is not connected." });

            var timeout = TimeSpan.FromSeconds(request.TimeoutSeconds <= 0 ? 30 : request.TimeoutSeconds);

            try
            {
                const string action = "Reset";
                object payload;

                if (conn.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                {
                    var type = request.Type ?? "Soft";
                    if (!ParseHelper.IsAny(type, "Hard", "Soft"))
                        return BadRequest(new { message = "OCPP 1.6 'type' must be 'Hard' or 'Soft'.", provided = request.Type });

                    payload = new { type = ParseHelper.NormalizeEnum(type, "Hard", "Soft") };
                }
                else
                {
                    var type = request.Type ?? "Immediate";
                    if (!ParseHelper.IsAny(type, "Immediate", "OnIdle"))
                        return BadRequest(new { message = "OCPP 2.0.1 'type' must be 'Immediate' or 'OnIdle'.", provided = request.Type });

                    payload = new { type = ParseHelper.NormalizeEnum(type, "Immediate", "OnIdle"), evseId = request.EvseId };
                }

                var resultJson = await conn.SendCallAsync(action, payload, timeout, ct);

                return Ok(new
                {
                    chargePointId = id,
                    protocol = conn.ProtocolVersion.ToString(),
                    action,
                    request = payload,
                    response = JsonDocument.Parse(resultJson).RootElement
                });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "Timed out waiting for charger response." });
            }
            catch (OcppCallErrorException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "Charger returned CALLERROR.",
                    errorCode = ex.ErrorCode,
                    description = ex.Message,
                    details = ParseHelper.TryParseOrEcho(ex.DetailsJson)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reset failed for CP={CP}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpPost("{id}/remoteStart")]
        public async Task<IActionResult> RemoteStartAsync(string id, [FromBody] RemoteStartDto dto, CancellationToken ct)
        {
            if (!_connections.TryGet(id, out var conn) || conn is null)
                return NotFound(new { message = $"Charge point '{id}' is not connected." });

            var timeout = TimeSpan.FromSeconds(dto.TimeoutSeconds <= 0 ? 30 : dto.TimeoutSeconds);

            try
            {
                if (conn.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                {
                    var payload = new
                    {
                        idTag = dto.IdTag ?? "DEMO",
                        connectorId = dto.ConnectorId ?? 0 // 0 or specific connector
                    };
                    var result = await conn.SendCallAsync("RemoteStartTransaction", payload, timeout, ct);
                    return Ok(new { protocol = "1.6", request = payload, response = JsonDocument.Parse(result).RootElement });
                }
                else
                {
                    var payload = new
                    {
                        evseId = dto.EvseId ?? 1,
                        remoteStartId = dto.RemoteStartId ?? Guid.NewGuid().ToString("N"),
                        idToken = new { idToken = dto.IdTag ?? "DEMO", type = "NoAuthorization" }
                    };
                    var result = await conn.SendCallAsync("RequestStartTransaction", payload, timeout, ct);
                    return Ok(new { protocol = "2.0.1", request = payload, response = JsonDocument.Parse(result).RootElement });
                }
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "Timeout" });
            }
        }

        [HttpPost("{id}/remoteStop")]
        public async Task<IActionResult> RemoteStopAsync(string id, [FromBody] RemoteStopDto dto, CancellationToken ct)
        {
            if (!_connections.TryGet(id, out var conn) || conn is null)
                return NotFound(new { message = $"Charge point '{id}' is not connected." });

            var timeout = TimeSpan.FromSeconds(dto.TimeoutSeconds <= 0 ? 30 : dto.TimeoutSeconds);

            try
            {
                if (conn.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                {
                    var payload = new { transactionId = dto.TransactionId16 ?? 0 };
                    var result = await conn.SendCallAsync("RemoteStopTransaction", payload, timeout, ct);
                    return Ok(new { protocol = "1.6", request = payload, response = JsonDocument.Parse(result).RootElement });
                }
                else
                {
                    var payload = new { transactionId = dto.TransactionId201 ?? "" };
                    var result = await conn.SendCallAsync("RequestStopTransaction", payload, timeout, ct);
                    return Ok(new { protocol = "2.0.1", request = payload, response = JsonDocument.Parse(result).RootElement });
                }
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "Timeout" });
            }
        }
    }
}