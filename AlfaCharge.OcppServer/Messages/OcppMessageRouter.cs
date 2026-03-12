using System;
using System.Text.Json;
using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.OcppServer.Contracts.AbstractFactory;
using AlfaCharge.OcppServer.Contracts.DTO;
using AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware;
using AlfaCharge.OcppServer.Contracts.DTO.Message201;
using AlfaCharge.OcppServer.Contracts.DTO.Messages;
using AlfaCharge.OcppServer.Helpers;
using AlfaCharge.OcppServer.Services;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.Extensions.Logging;

namespace AlfaCharge.OcppServer.Messages
{

    public sealed class OcppMessageRouter
    {
        private readonly IOcppHandlerFactory _handlerFactory;
        private readonly OcppConnection _connection;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOcppLogWriter _logWriter;
        private CancellationToken cancellationToken => _connection.Cancellation;

        public OcppMessageRouter(IOcppHandlerFactory handlerFactory,
            OcppConnection connection,
            JsonSerializerOptions jsonOptions,
            ILogger logger, 
            IServiceProvider serviceProvider,
            IOcppLogWriter logWriter)
        {
            _handlerFactory = handlerFactory;
            _connection = connection;
            _jsonOptions = jsonOptions;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _logWriter = logWriter;
        }

        public async Task<string?> RouteAsync(string json)
        {
            JsonElement[] arr;
            try
            {
                arr = JsonSerializer.Deserialize<JsonElement[]>(json, _jsonOptions)!;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid OCPP frame (not an array). CP={CP}", _connection.ChargePointId);
                return null;
            }

            if (arr.Length == 0 || arr[0].ValueKind != JsonValueKind.Number)
                return null;

            var messageType = arr[0].GetInt32();
            var inboundMsgId = arr.Length > 1 ? arr[1].GetString() : null;
            var inboundAction = (messageType == 2 && arr.Length > 2) ? arr[2].GetString() : null;

            // START SCOPE EARLY so *all* logger entries below include cpId/msgId/(action)
            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["cpId"] = _connection.ChargePointId,
                ["msgId"] = inboundMsgId ?? "-",   // safe placeholder
                ["action"] = inboundAction         // optional but useful
            }))
            {
                // Inbound frame persisted/audited in your DB log (scope doesn’t affect this)
                await _logWriter.LogAsync(
                    _connection.ChargePointId,
                    direction: "inbound",
                    messageTypeId: messageType,
                    messageId: inboundMsgId,
                    action: inboundAction,
                    payloadJson: json);

                switch (messageType)
                {
                    case 2: // CALL
                        return await HandleCallAsync(arr);

                    case 3: // CALLRESULT
                        HandleCallResult(arr);
                        return null;

                    case 4: // CALLERROR
                        HandleCallError(arr);
                        return null;

                    default:
                        _logger.LogWarning("Unknown MessageType {Type} from CP={CP}", messageType, _connection.ChargePointId);
                        return null;
                }
            }
        }

        private async Task<string?> HandleCallAsync(JsonElement[] arr)
        {
            if (arr.Length < 4)
            {
                _logger.LogWarning("Malformed CALL frame. CP={CP}", _connection.ChargePointId);
                var issueMessageId = arr.Length > 1 ? arr[1].GetString() ?? Guid.NewGuid().ToString("N") : Guid.NewGuid().ToString("N");
                var error = new object[] { 4, issueMessageId, OcppErrorCodes.ProtocolError, "Malformed CALL frame", new { } };
                await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 4, issueMessageId, null,
                    JsonSerializer.Serialize(new { code = "ProtocolError", description = "Malformed CALL frame" }, _jsonOptions), "400");
                //Return a CALLERROR with ProtocolError.
                return JsonSerializer.Serialize(error, _jsonOptions);
            }

            var messageId = arr[1].GetString()!;
            var actionRaw = arr[2].GetString();
            var payload = arr[3];
            var action = actionRaw?.Trim();
            var normalized = action?.ToLowerInvariant();

            //It gives the CP a clear reason for rejection, If action is null/empty
            if (string.IsNullOrWhiteSpace(normalized))
            {
                var err = new object[] { 4, messageId, OcppErrorCodes.ProtocolError, "Missing action", new { } };
                await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 4, messageId, null,
                    JsonSerializer.Serialize(new { code = "ProtocolError", description = "Missing action" }, _jsonOptions), "400");
                return JsonSerializer.Serialize(err, _jsonOptions);
            }

            try
            {
                switch (normalized)
                {
                    //case "bootnotification":
                    //    {
                    //        var req = JsonSerializer.Deserialize<BootNotificationRequest>(payload.GetRawText(), _jsonOptions)!;
                    //        var handler = _handlerFactory.CreateBootNotificationHandler();
                    //        var resp = await handler.HandleAsync(req);

                    //        //log the details of the response
                    //        await _logWriter.LogAsync(
                    //            _connection.ChargePointId,
                    //            direction: "outbound",
                    //            messageTypeId: 3,
                    //            messageId: messageId,
                    //            action: action,
                    //            payloadJson: JsonSerializer.Serialize(resp, _jsonOptions),
                    //            resultCode: "ok");

                    //        return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                    //    }
                    case "bootnotification":
                        {
                            if (_connection.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                            {
                                // 1.6 ? normalize into your 2.x BootNotificationRequest
                                var v16 = JsonSerializer.Deserialize<BootNotificationRequest16>(payload.GetRawText(), _jsonOptions)!;

                                var unified = new BootNotificationRequest
                                {
                                    Reason = null, // 1.6 has no 'reason'
                                    ChargingStation = new ChargingStationInfo
                                    {
                                        Model = v16.chargePointModel,
                                        VendorName = v16.chargePointVendor,
                                        SerialNumber = v16.chargeBoxSerialNumber ?? v16.meterSerialNumber,
                                        FirmwareVersion = v16.firmwareVersion
                                    }
                                };

                                var handler = _handlerFactory.CreateBootNotificationHandler();
                                var resp = await handler.HandleAsync(unified);
                                await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, "bootnotification",
                                    JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                                return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                            }
                            else
                            {
                                // 2.0.1/2.1: your existing 2.x class
                                var req = JsonSerializer.Deserialize<BootNotificationRequest>(payload.GetRawText(), _jsonOptions)!;
                                var handler = _handlerFactory.CreateBootNotificationHandler();
                                var resp = await handler.HandleAsync(req);
                                await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, "bootnotification",
                                    JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                                return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                            }
                        }

                    case "heartbeat":
                        {
                            var handler = _handlerFactory.CreateHeartbeatHandler();
                            // Heartbeat request is empty in 1.6 and 2.0.1
                            var resp = await handler.HandleAsync();

                            //reuse common method to build CALLRESULT and log it
                            return await BuildCallResult(resp, messageId, "heartbeat");

                            //log the details of the response
                            ////await _logWriter.LogAsync(
                            ////    _connection.ChargePointId,
                            ////    direction: "outbound",
                            ////    messageTypeId: 3,
                            ////    messageId: messageId,
                            ////    action: action,
                            ////    payloadJson: JsonSerializer.Serialize(resp, _jsonOptions),
                            ////    resultCode: "ok");

                            ////return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "authorize":
                        {
                            var req = JsonSerializer.Deserialize<AuthorizeRequest>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.CreateAuthorizeHandler();
                            var resp = await handler.HandleAsync(req);

                            //log the details of the response
                            await _logWriter.LogAsync(
                                _connection.ChargePointId,
                                direction: "outbound",
                                messageTypeId: 3,
                                messageId: messageId,
                                action: action,
                                payloadJson: JsonSerializer.Serialize(resp, _jsonOptions),
                                resultCode: "ok");

                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }


                    case "statusnotification":
                        {
                            var cpId = _connection.ChargePointId;
                            var handler = _handlerFactory.CreateStatusNotificationHandler();

                            if (_connection.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                            {
                                var req = JsonSerializer.Deserialize<Ocpp16StatusNotificationRequest>(payload.GetRawText(), _jsonOptions)!;
                                var resp = await handler.Handle16Async(cpId, req);

                                await _logWriter.LogAsync(cpId, "outbound", 3, messageId, action,
                                    JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                                return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                            }
                            else
                            {
                                var req = JsonSerializer.Deserialize<Ocpp201StatusNotificationRequest>(payload.GetRawText(), _jsonOptions)!;
                                var resp = await handler.Handle201Async(cpId, req);

                                await _logWriter.LogAsync(cpId, "outbound", 3, messageId, action,
                                    JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                                return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                            }
                        }

                    case "starttransaction":
                        {
                            var req = JsonSerializer.Deserialize<Ocpp16StartTransactionRequest>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.Create16TransactionHandler();
                            var resp = await handler.HandleStartAsync(_connection.ChargePointId, req);

                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "stoptransaction":
                        {
                            var req = JsonSerializer.Deserialize<Ocpp16StopTransactionRequest>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.Create16TransactionHandler();
                            var resp = await handler.HandleStopAsync(_connection.ChargePointId, req);

                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "metervalues":
                        {
                            var req = JsonSerializer.Deserialize<Ocpp16MeterValuesRequest>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.Create16TransactionHandler();
                            var resp = await handler.HandleMeterValuesAsync(_connection.ChargePointId, req);

                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "transactionevent":
                        {
                            var req = JsonSerializer.Deserialize<Ocpp201TransactionEventRequest>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.Create201TransactionHandler();
                            var resp = await handler.HandleEventAsync(_connection.ChargePointId, req);

                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }


                    // ... Configuration normalized CP ? CSMS
                    case "diagnosticsstatusnotification": // OCPP 1.6
                        {
                            var req = JsonSerializer.Deserialize<DiagnosticsStatusNotificationReq16>(payload.GetRawText(), _jsonOptions)!;
                            var handler = _handlerFactory.CreateDiagnosticsFirmwareOps16();
                            await handler.HandleDiagnosticsStatusNotificationAsync(_connection.ChargePointId, req, cancellationToken);
                            var resp = new { }; // empty payload per 1.6
                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "firmwarestatusnotification": // 1.6 or 2.x (both use same action name)
                        {
                            if (_connection.ProtocolVersion == OcppProtocolVersion.Ocpp16)
                            {
                                var req16 = JsonSerializer.Deserialize<FirmwareStatusNotificationReq16>(payload.GetRawText(), _jsonOptions)!;
                                var h16 = _handlerFactory.CreateDiagnosticsFirmwareOps16();
                                await h16.HandleFirmwareStatusNotificationAsync(_connection.ChargePointId, req16, cancellationToken);
                            }
                            else
                            {
                                var req201 = JsonSerializer.Deserialize<FirmwareStatusNotificationReq201>(payload.GetRawText(), _jsonOptions)!;
                                var h201 = _handlerFactory.CreateDiagnosticsFirmwareOps201();
                                await h201.HandleFirmwareStatusNotificationAsync(_connection.ChargePointId, req201, cancellationToken);
                            }
                            var resp = new { };
                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "logstatusnotification": // OCPP 2.0.1/2.1 only
                        {
                            var req = JsonSerializer.Deserialize<LogStatusNotificationReq201>(payload.GetRawText(), _jsonOptions)!;
                            var h201 = _handlerFactory.CreateDiagnosticsFirmwareOps201();
                            await h201.HandleLogStatusNotificationAsync(_connection.ChargePointId, req, cancellationToken);
                            var resp = new { };
                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    case "notifyreport": // OCPP 2.0.1/2.1 (GetBaseReport stream)
                        {
                            var req = JsonSerializer.Deserialize<NotifyReportReq201>(payload.GetRawText(), _jsonOptions)!;
                            var h201 = _handlerFactory.CreateConfigurationOps201();
                            await h201.HandleNotifyReportAsync(_connection.ChargePointId, req, cancellationToken);
                            var resp = new { };
                            await _logWriter.LogAsync(_connection.ChargePointId, "outbound", 3, messageId, action,
                                JsonSerializer.Serialize(resp, _jsonOptions), "ok");
                            return JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
                        }

                    default:
                        {
                            // Unknown action -> CALLERROR
                            var desc = $"Action '{actionRaw}' not supported";
                            var error = new object[] { 4, messageId, OcppErrorCodes.NotImplemented, desc, new { } };

                            // log the details of the response
                            var errPayload = new { code = OcppErrorCodes.NotImplemented, description = desc };
                            await _logWriter.LogAsync(
                                _connection.ChargePointId,
                                direction: "outbound",
                                messageTypeId: 4,
                                messageId: messageId,
                                action: action,
                                payloadJson: JsonSerializer.Serialize(errPayload, _jsonOptions),
                                resultCode: "501");

                            return JsonSerializer.Serialize(error, _jsonOptions);
                        }
                }
            }
            catch (Exception ex)
            {
                // Internal errors -> CALLERROR InternalError
                var error = new object[] { 4, messageId, OcppErrorCodes.InternalError, ex.Message, new { } };
                return JsonSerializer.Serialize(error, _jsonOptions);
            }
        }

        private void HandleCallResult(JsonElement[] arr)
        {
            if (arr.Length < 3) return;
            var messageId = arr[1].GetString()!;
            var payload = arr[2].GetRawText();
            if (_connection.TryComplete(messageId, payload))
            {
                _logger.LogDebug("CALLRESULT matched pending id={Id} CP={CP}", messageId, _connection.ChargePointId);
            }
            else
            {
                _logger.LogWarning("Unmatched CALLRESULT id={Id} CP={CP}", messageId, _connection.ChargePointId);
            }
        }

        private void HandleCallError(JsonElement[] arr)
        {
            if (arr.Length < 5) return;
            var messageId = arr[1].GetString()!;
            var errorCode = arr[2].GetString() ?? OcppErrorCodes.GenericError;
            var description = arr[3].GetString() ?? "Unknown error";
            var details = arr[4].GetRawText();

            var ex = new OcppCallErrorException(messageId, errorCode, description, details);
            if (_connection.TryFail(messageId, ex))
            {
                _logger.LogDebug("CALLERROR matched pending id={Id} CP={CP}", messageId, _connection.ChargePointId);
            }
            else
            {
                _logger.LogWarning("Unmatched CALLERROR id={Id} CP={CP}", messageId, _connection.ChargePointId);
            }
        }

        private async Task<string> BuildCallResult(object resp, string messageId, string? actionForLog = null)
        {
            var json = JsonSerializer.Serialize(new object[] { 3, messageId, resp }, _jsonOptions);
            await _logWriter.LogAsync(
                _connection.ChargePointId,
                "outbound",
                3,
                messageId,
                actionForLog,
                JsonSerializer.Serialize(resp, _jsonOptions),
                "ok");
            return json;
        }
    }
}