using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargePointDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly OcppConnectionManager _connections;

        public ChargePointDetailsController(ApplicationDbContext db, OcppConnectionManager connections)
        {
            _db = db;
            _connections = connections;
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(string id, CancellationToken ct)
        {
            // Load your ChargePoint row (adjust to your real EF entity instead of FromSqlRaw)
            var cp = await _db.Set<dynamic>()
                .FromSqlRaw("SELECT TOP 1 * FROM ChargePoints WHERE charge_point_id = {0}", id)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (cp is null)
                return NotFound(new { message = $"Charge point '{id}' not found." });

            Guid cpDbId = Guid.Empty;
            try { cpDbId = (Guid)cp.id; } catch { }

            var connectors = await _db.Connectors
                .Where(x => x.ChargePointDbId == cpDbId)
                .OrderBy(x => x.ConnectorNumber)
                .Select(x => new
                {
                    id = x.Id,
                    number = x.ConnectorNumber,
                    status = x.Status,
                    operationalStatus = x.OperationalStatus,
                    errorCode = x.ErrorCode,
                    lastStatusTimestamp = x.LastStatusTimestamp
                })
                .ToListAsync(ct);

            var lastSeen = await _db.StatusHistories
                .Where(x => x.ChargePointId == id)
                .OrderByDescending(x => x.OccurredAt)
                .Select(x => x.OccurredAt)
                .FirstOrDefaultAsync(ct);

            var online = _connections.TryGet(id, out var conn) && conn is not null;
            var protocol = online ? conn!.ProtocolVersion.ToString() : null;

            var dto = new
            {
                chargePointId = id,
                stationName = cp.station_name,              // adjust to your schema
                locationId = cp.location_id,
                model = cp.model,
                firmwareVersion = cp.firmware_version,
                serialNumber = cp.serial_number,
                online,
                protocol,
                lastSeen,
                connectors
            };

            return Ok(dto);
        }
    }
}