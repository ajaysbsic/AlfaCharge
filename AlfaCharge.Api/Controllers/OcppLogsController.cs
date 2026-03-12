using AlfaCharge.Api.DTO;
using AlfaCharge.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers;

/// <summary>
/// API for OCPP logs access.
/// </summary>
[Route("api/ocpp/logs")]
[ApiController]
public class OcppLogsController : ControllerBase
{
    private readonly ILogger<OcppLogsController> _logger;
    private readonly ApplicationDbContext _db;

    public OcppLogsController(ILogger<OcppLogsController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Get paged OCPP logs with filtering.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<OcppLogListDto>>> GetLogs(
        [FromQuery] OcppLogQueryDto query,
        CancellationToken ct)
    {
        try
        {
            var dbQuery = _db.OcppLogs.AsQueryable();

            // Filter by charge point
            if (!string.IsNullOrWhiteSpace(query.ChargePointId))
            {
                dbQuery = dbQuery.Where(l => l.ChargePointId == query.ChargePointId);
            }

            // Filter by action
            if (!string.IsNullOrWhiteSpace(query.Action))
            {
                dbQuery = dbQuery.Where(l => l.Action == query.Action);
            }

            // Filter by direction
            if (!string.IsNullOrWhiteSpace(query.Direction))
            {
                dbQuery = dbQuery.Where(l => l.Direction == query.Direction);
            }

            // Filter by date range
            if (query.FromDate.HasValue)
            {
                dbQuery = dbQuery.Where(l => l.Timestamp >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                dbQuery = dbQuery.Where(l => l.Timestamp <= query.ToDate.Value);
            }

            // Search in payload or action
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                dbQuery = dbQuery.Where(l =>
                    (l.Action != null && l.Action.ToLower().Contains(search)) ||
                    l.ChargePointId.ToLower().Contains(search) ||
                    l.PayloadJson.ToLower().Contains(search));
            }

            var totalCount = await dbQuery.CountAsync(ct);

            // Sorting
            dbQuery = query.SortBy?.ToLower() switch
            {
                "chargepointid" => query.SortDescending 
                    ? dbQuery.OrderByDescending(l => l.ChargePointId) 
                    : dbQuery.OrderBy(l => l.ChargePointId),
                "action" => query.SortDescending 
                    ? dbQuery.OrderByDescending(l => l.Action) 
                    : dbQuery.OrderBy(l => l.Action),
                "direction" => query.SortDescending 
                    ? dbQuery.OrderByDescending(l => l.Direction) 
                    : dbQuery.OrderBy(l => l.Direction),
                _ => dbQuery.OrderByDescending(l => l.Timestamp) // Default: newest first
            };

            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(l => new OcppLogListDto
                {
                    Id = l.Id,
                    ChargePointId = l.ChargePointId,
                    Direction = l.Direction,
                    MessageTypeId = l.MessageTypeId,
                    MessageId = l.MessageId,
                    Action = l.Action,
                    PayloadJson = l.PayloadJson,
                    ResultCode = l.ResultCode,
                    Timestamp = l.Timestamp
                })
                .ToListAsync(ct);

            return Ok(new PagedResultDto<OcppLogListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OCPP logs");
            return StatusCode(500, "Error retrieving OCPP logs");
        }
    }

    /// <summary>
    /// Get single log entry by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OcppLogListDto>> GetLog(Guid id, CancellationToken ct)
    {
        var log = await _db.OcppLogs.FindAsync([id], ct);
        if (log is null)
            return NotFound();

        return Ok(new OcppLogListDto
        {
            Id = log.Id,
            ChargePointId = log.ChargePointId,
            Direction = log.Direction,
            MessageTypeId = log.MessageTypeId,
            MessageId = log.MessageId,
            Action = log.Action,
            PayloadJson = log.PayloadJson,
            ResultCode = log.ResultCode,
            Timestamp = log.Timestamp
        });
    }

    /// <summary>
    /// Get distinct actions for filtering.
    /// </summary>
    [HttpGet("actions")]
    public async Task<ActionResult<List<string>>> GetDistinctActions(CancellationToken ct)
    {
        var actions = await _db.OcppLogs
            .Where(l => l.Action != null)
            .Select(l => l.Action!)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync(ct);

        return Ok(actions);
    }

    /// <summary>
    /// Get distinct charge point IDs for filtering.
    /// </summary>
    [HttpGet("chargepoints")]
    public async Task<ActionResult<List<string>>> GetDistinctChargePoints(CancellationToken ct)
    {
        var chargePoints = await _db.OcppLogs
            .Select(l => l.ChargePointId)
            .Distinct()
            .OrderBy(cp => cp)
            .ToListAsync(ct);

        return Ok(chargePoints);
    }
}
