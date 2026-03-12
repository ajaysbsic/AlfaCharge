using AlfaCharge.Api.DTO;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers;

/// <summary>
/// Admin API for station management with paging, filtering, and OCPP actions.
/// </summary>
[Route("api/admin/stations")]
[ApiController]
public class AdminStationsController : ControllerBase
{
    private readonly ILogger<AdminStationsController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly OcppConnectionManager _connectionManager;
    private readonly IRemoteOps16 _remoteOps16;
    private readonly ILocalAuthListOps16 _localListOps16;
    private readonly IReservationOps16 _reservationOps16;
    private readonly IChargingProfileOps16 _chargingProfileOps16;
    private readonly ITriggersOps16 _triggersOps16;
    private readonly IDiagnosticsFirmwareOps16 _diagFirmwareOps16;

    public AdminStationsController(
        ILogger<AdminStationsController> logger,
        ApplicationDbContext db,
        OcppConnectionManager connectionManager,
        IRemoteOps16 remoteOps16,
        ILocalAuthListOps16 localListOps16,
        IReservationOps16 reservationOps16,
        IChargingProfileOps16 chargingProfileOps16,
        ITriggersOps16 triggersOps16,
        IDiagnosticsFirmwareOps16 diagFirmwareOps16)
    {
        _logger = logger;
        _db = db;
        _connectionManager = connectionManager;
        _remoteOps16 = remoteOps16;
        _localListOps16 = localListOps16;
        _reservationOps16 = reservationOps16;
        _chargingProfileOps16 = chargingProfileOps16;
        _triggersOps16 = triggersOps16;
        _diagFirmwareOps16 = diagFirmwareOps16;
    }

    /// <summary>
    /// Get paged list of stations with filtering and search.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<StationListDto>>> GetStations(
        [FromQuery] StationQueryDto query,
        CancellationToken ct)
    {
        try
        {
            var dbQuery = _db.ChargePoints.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                dbQuery = dbQuery.Where(s =>
                    EF.Functions.Like(s.ChargePointId.ToLower(), $"%{search}%") ||
                    (s.Station_name != null && EF.Functions.Like(s.Station_name.ToLower(), $"%{search}%")));
            }

            // Filter by location
            if (!string.IsNullOrWhiteSpace(query.LocationId))
            {
                dbQuery = dbQuery.Where(s => s.LocationId == query.LocationId);
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                dbQuery = dbQuery.Where(s => s.Status == query.Status);
            }

            // Filter by firmware
            if (!string.IsNullOrWhiteSpace(query.FirmwareVersion))
            {
                dbQuery = dbQuery.Where(s => s.FirmwareVersion == query.FirmwareVersion);
            }

            var totalCount = await dbQuery.CountAsync(ct);

            // Sorting
            dbQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending 
                    ? dbQuery.OrderByDescending(s => s.Station_name) 
                    : dbQuery.OrderBy(s => s.Station_name),
                "status" => query.SortDescending 
                    ? dbQuery.OrderByDescending(s => s.Status) 
                    : dbQuery.OrderBy(s => s.Status),
                "location" => query.SortDescending 
                    ? dbQuery.OrderByDescending(s => s.LocationId) 
                    : dbQuery.OrderBy(s => s.LocationId),
                _ => dbQuery.OrderBy(s => s.ChargePointId)
            };

            // Paging - select intermediate data
            var stationsData = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new
                {
                    s.Id,
                    s.ChargePointId,
                    s.Station_name,
                    s.LocationId,
                    s.Status,
                    s.Model,
                    s.FirmwareVersion,
                    s.SerialNumber
                })
                .ToListAsync(ct);

            // Map to DTOs and add connection status
            var items = stationsData.Select(s => new StationListDto
            {
                Id = s.Id,
                ChargePointId = s.ChargePointId,
                StationName = s.Station_name,
                LocationId = s.LocationId,
                Status = s.Status,
                Model = s.Model,
                FirmwareVersion = s.FirmwareVersion,
                SerialNumber = s.SerialNumber,
                IsConnected = _connectionManager.IsConnected(s.ChargePointId)
            }).ToList();

            // Get connector counts
            var cpIds = items.Select(i => i.Id).ToList();
            var connectorCounts = await _db.Connectors
                .Where(c => cpIds.Contains(c.ChargePointDbId))
                .GroupBy(c => c.ChargePointDbId)
                .Select(g => new { ChargePointDbId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            foreach (var item in items)
            {
                item.ConnectorCount = connectorCounts
                    .FirstOrDefault(c => c.ChargePointDbId == item.Id)?.Count ?? 0;
            }

            return Ok(new PagedResultDto<StationListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stations");
            return StatusCode(500, "Error retrieving stations");
        }
    }

    /// <summary>
    /// Get station details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StationDetailDto>> GetStation(Guid id, CancellationToken ct)
    {
        var station = await _db.ChargePoints.FindAsync([id], ct);
        if (station is null)
            return NotFound();

        var connectors = await _db.Connectors
            .Where(c => c.ChargePointDbId == id)
            .Select(c => new ConnectorDto
            {
                Id = c.Id,
                ConnectorNumber = c.ConnectorNumber,
                Status = c.Status.ToString(),
                ErrorCode = c.ErrorCode,
                LastStatusTimestamp = c.LastStatusTimestamp,
                PowerType = c.PowerType.ToString(),
                PowerKw = c.PowerKw
            })
            .ToListAsync(ct);

        var recentLogs = await _db.OcppLogs
            .Where(l => l.ChargePointId == station.ChargePointId)
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .Select(l => new RecentActivityDto
            {
                Timestamp = l.Timestamp,
                Type = l.Action ?? "Unknown",
                Description = $"{l.Direction}: {l.Action}"
            })
            .ToListAsync(ct);

        return Ok(new StationDetailDto
        {
            Id = station.Id,
            ChargePointId = station.ChargePointId,
            StationName = station.Station_name,
            LocationId = station.LocationId,
            Status = station.Status,
            Model = station.Model,
            FirmwareVersion = station.FirmwareVersion,
            SerialNumber = station.SerialNumber,
            IsConnected = _connectionManager.IsConnected(station.ChargePointId),
            ConnectorCount = connectors.Count,
            Connectors = connectors,
            RecentActivity = recentLogs
        });
    }

    /// <summary>
    /// Create a new station.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StationListDto>> CreateStation(
        [FromBody] StationUpsertDto dto,
        CancellationToken ct)
    {
        if (await _db.ChargePoints.AnyAsync(s => s.ChargePointId == dto.ChargePointId, ct))
            return Conflict("Station with this ChargePointId already exists");

        var station = new ChargePoint
        {
            Id = Guid.NewGuid(),
            ChargePointId = dto.ChargePointId,
            Station_name = dto.StationName,
            LocationId = dto.LocationId,
            Model = dto.Model,
            FirmwareVersion = dto.FirmwareVersion,
            SerialNumber = dto.SerialNumber,
            Status = "Available"
        };

        _db.ChargePoints.Add(station);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetStation), new { id = station.Id }, new StationListDto
        {
            Id = station.Id,
            ChargePointId = station.ChargePointId,
            StationName = station.Station_name,
            LocationId = station.LocationId,
            Status = station.Status
        });
    }

    /// <summary>
    /// Update a station.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateStation(Guid id, [FromBody] StationUpsertDto dto, CancellationToken ct)
    {
        var station = await _db.ChargePoints.FindAsync([id], ct);
        if (station is null)
            return NotFound();

        station.Station_name = dto.StationName;
        station.LocationId = dto.LocationId;
        station.Model = dto.Model;
        station.FirmwareVersion = dto.FirmwareVersion;
        station.SerialNumber = dto.SerialNumber;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Delete a station.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteStation(Guid id, CancellationToken ct)
    {
        var station = await _db.ChargePoints.FindAsync([id], ct);
        if (station is null)
            return NotFound();

        _db.ChargePoints.Remove(station);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    #region OCPP Actions

    /// <summary>
    /// Remote start transaction.
    /// </summary>
    [HttpPost("{chargePointId}/actions/remote-start")]
    public async Task<ActionResult<OcppActionResultDto>> RemoteStart(
        string chargePointId,
        [FromBody] RemoteStartDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _remoteOps16.RemoteStartTransactionAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoteStart failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Remote stop transaction.
    /// </summary>
    [HttpPost("{chargePointId}/actions/remote-stop")]
    public async Task<ActionResult<OcppActionResultDto>> RemoteStop(
        string chargePointId,
        [FromBody] RemoteStopDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _remoteOps16.RemoteStopTransactionAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoteStop failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Reset station.
    /// </summary>
    [HttpPost("{chargePointId}/actions/reset")]
    public async Task<ActionResult<OcppActionResultDto>> Reset(
        string chargePointId,
        [FromBody] ResetDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _remoteOps16.ResetAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reset failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Trigger message.
    /// </summary>
    [HttpPost("{chargePointId}/actions/trigger-message")]
    public async Task<ActionResult<OcppActionResultDto>> TriggerMessage(
        string chargePointId,
        [FromBody] TriggerMessageDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _triggersOps16.TriggerMessageAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TriggerMessage failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get local list version.
    /// </summary>
    [HttpGet("{chargePointId}/actions/local-list-version")]
    public async Task<ActionResult<OcppActionResultDto>> GetLocalListVersion(
        string chargePointId,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _localListOps16.GetLocalListVersionAsync(chargePointId, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetLocalListVersion failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Send local list.
    /// </summary>
    [HttpPost("{chargePointId}/actions/send-local-list")]
    public async Task<ActionResult<OcppActionResultDto>> SendLocalList(
        string chargePointId,
        [FromBody] SendLocalListDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _localListOps16.SendLocalListAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendLocalList failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Reserve now.
    /// </summary>
    [HttpPost("{chargePointId}/actions/reserve-now")]
    public async Task<ActionResult<OcppActionResultDto>> ReserveNow(
        string chargePointId,
        [FromBody] ReserveNowDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _reservationOps16.ReserveNowAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReserveNow failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Cancel reservation.
    /// </summary>
    [HttpPost("{chargePointId}/actions/cancel-reservation")]
    public async Task<ActionResult<OcppActionResultDto>> CancelReservation(
        string chargePointId,
        [FromBody] CancelReservationDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _reservationOps16.CancelReservationAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CancelReservation failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Clear charging profile.
    /// </summary>
    [HttpPost("{chargePointId}/actions/clear-charging-profile")]
    public async Task<ActionResult<OcppActionResultDto>> ClearChargingProfile(
        string chargePointId,
        [FromBody] ClearChargingProfileDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _chargingProfileOps16.ClearChargingProfileAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ClearChargingProfile failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Set charging profile.
    /// </summary>
    [HttpPost("{chargePointId}/actions/set-charging-profile")]
    public async Task<ActionResult<OcppActionResultDto>> SetChargingProfile(
        string chargePointId,
        [FromBody] SetChargingProfileDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _chargingProfileOps16.SetChargingProfileAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetChargingProfile failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get composite schedule.
    /// </summary>
    [HttpPost("{chargePointId}/actions/get-composite-schedule")]
    public async Task<ActionResult<OcppActionResultDto>> GetCompositeSchedule(
        string chargePointId,
        [FromBody] GetCompositeScheduleDto dto,
        CancellationToken ct)
    {
        try
        {
            if (!_connectionManager.IsConnected(chargePointId))
                return BadRequest(new OcppActionResultDto { Success = false, Message = "Station is not connected" });

            var result = await _chargingProfileOps16.GetCompositeScheduleAsync(chargePointId, dto, ct);
            return Ok(new OcppActionResultDto { Success = true, Response = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCompositeSchedule failed for {ChargePointId}", chargePointId);
            return Ok(new OcppActionResultDto { Success = false, Message = ex.Message });
        }
    }

    #endregion
}
