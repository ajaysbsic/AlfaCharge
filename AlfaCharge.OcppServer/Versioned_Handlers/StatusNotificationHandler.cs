using System.Text.Json;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO;
using AlfaCharge.OcppServer.Hubs;
using AlfaCharge.OcppServer.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.OcppServer.Versioned_Handlers
{

    public class StatusNotificationHandler : IStatusNotificationHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<OcppEventsHub> _hub;
        private readonly IOcppLogWriter _logWriter;
        private readonly JsonSerializerOptions _json;

        public StatusNotificationHandler(ApplicationDbContext db,
                                         IHubContext<OcppEventsHub> hub,
                                         IOcppLogWriter logWriter,
                                         JsonSerializerOptions json)
        {
            _db = db;
            _hub = hub;
            _logWriter = logWriter;
            _json = json;
        }

        public async Task<object> Handle16Async(string chargePointId, Ocpp16StatusNotificationRequest req)
        {
            // 1) Load the ChargePoint row to obtain its DB Id (for Connector relation).
            //var cp = await _db.Set<dynamic>()
            //    .FromSqlRaw("SELECT TOP 1 * FROM ChargePoints WHERE charge_point_id = {0}", chargePointId)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();

            var cp = await _db.ChargePoints
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChargePointId == chargePointId);

            // Resolve cpDbId safely
            Guid cpDbId = cp?.Id ?? Guid.Empty;

            // 2) Upsert Connector row for the reported connectorId.
            var connector = await _db.Connectors
                .Where(x => x.ChargePointDbId == cpDbId && x.ConnectorNumber == req.ConnectorId)
                .FirstOrDefaultAsync();

            if (connector == null)
            {
                connector = new Connector
                {
                    Id = Guid.NewGuid(),
                    ChargePointDbId = cpDbId,
                    ConnectorNumber = req.ConnectorId
                };
                _db.Connectors.Add(connector);
            }

            // 3) Map 1.6 runtime status into your enum and update connector runtime fields.
            connector.Status = Map16RuntimeStatus(req.Status.ToString());           // enum assignment
            connector.ErrorCode = req.ErrorCode == "NoError" ? null : req.ErrorCode;
            connector.LastStatusTimestamp = req.Timestamp ?? DateTimeOffset.UtcNow;
            connector.UpdatedDate = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            // 4) Persist a StatusHistory entry (runtime), storing the enum as string.
            _db.StatusHistories.Add(new StatusHistory
            {
                Id = Guid.NewGuid(),
                ChargePointId = chargePointId,
                ConnectorDbId = connector.Id,
                StatusType = "runtime",
                Status = connector.Status.ToString(), // store the enum as string
                ErrorCode = connector.ErrorCode,
                OccurredAt = connector.LastStatusTimestamp ?? DateTimeOffset.UtcNow,
                DetailsJson = JsonSerializer.Serialize(req, _json)
            });

            await _db.SaveChangesAsync();

            // 5) SignalR broadcast: notify UI subscribers of the runtime state change.
            var payload = new
            {
                chargePointId,
                connectorId = req.ConnectorId,
                status = connector.Status.ToString(),
                errorCode = connector.ErrorCode,
                timestamp = connector.LastStatusTimestamp
            };

            await _hub.Clients.Group($"cp:{chargePointId}").SendAsync("statusUpdated", payload);
            await _hub.Clients.All.SendAsync("statusUpdated", payload);

            // 6) Return empty payload per OCPP 1.6 StatusNotification response.
            return new Ocpp16StatusNotificationResponse();
        }

        public async Task<object> Handle201Async(string chargePointId, Ocpp201StatusNotificationRequest req)
        {
            // 1) Load the ChargePoint row to obtain its DB Id.
            //var cp = await _db.Set<dynamic>()
            //    .FromSqlRaw("SELECT TOP 1 * FROM ChargePoints WHERE charge_point_id = {0}", chargePointId)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
            var cp = await _db.ChargePoints
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChargePointId == chargePointId);


            // Resolve cpDbId safely
            Guid cpDbId = cp?.Id ?? Guid.Empty;

            // 2) Upsert Connector row using connector dimension (default 0 if not provided).
            var connectorNumber = req.ConnectorId ?? 0;

            var connector = await _db.Connectors
                .Where(x => x.ChargePointDbId == cpDbId && x.ConnectorNumber == connectorNumber)
                .FirstOrDefaultAsync();

            if (connector == null)
            {
                connector = new Connector
                {
                    Id = Guid.NewGuid(),
                    ChargePointDbId = cpDbId,
                    ConnectorNumber = connectorNumber
                };
                _db.Connectors.Add(connector);
            }

            // 3) OCPP 2.0.1 StatusNotification conveys AVAILABILITY (Operative/Inoperative).
            connector.OperationalStatus = (req.Status ?? "Operative").ToLowerInvariant(); // string
            connector.LastStatusTimestamp = req.Timestamp ?? DateTimeOffset.UtcNow;
            connector.UpdatedDate = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            // 4) Persist a StatusHistory entry (availability) using the availability string.
            _db.StatusHistories.Add(new StatusHistory
            {
                Id = Guid.NewGuid(),
                ChargePointId = chargePointId,
                ConnectorDbId = connector.Id,
                StatusType = "availability",
                Status = connector.OperationalStatus!, // string: "operative" / "inoperative"
                OccurredAt = connector.LastStatusTimestamp ?? DateTimeOffset.UtcNow,
                DetailsJson = JsonSerializer.Serialize(req, _json)
            });

            await _db.SaveChangesAsync();

            // 5) Broadcast availability change to UI.
            var payload = new
            {
                chargePointId,
                evseId = req.EvseId,
                connectorId = connectorNumber,
                operationalStatus = connector.OperationalStatus,
                timestamp = connector.LastStatusTimestamp
            };

            await _hub.Clients.Group($"cp:{chargePointId}").SendAsync("availabilityUpdated", payload);
            await _hub.Clients.All.SendAsync("availabilityUpdated", payload);

            // 6) Return empty payload per OCPP 2.0.1 StatusNotification response.
            return new Ocpp201StatusNotificationResponse();
        }


        private static ConnectorStatus Map16RuntimeStatus(string status)
        {
            switch ((status ?? "Unknown").Trim().ToLowerInvariant())
            {
                case "available":
                    return ConnectorStatus.Available;

                case "charging":
                    return ConnectorStatus.Charging;

                case "faulted":
                    return ConnectorStatus.Faulted;

                // Group these as temporarily unavailable (refine later if we extend enum)
                case "preparing":
                case "finishing":
                case "reserved":
                case "suspendedev":
                case "suspendedevse":
                case "unavailable":
                    return ConnectorStatus.Unavailable;

                default:
                    return ConnectorStatus.Unavailable;
            }
        }
    }
}