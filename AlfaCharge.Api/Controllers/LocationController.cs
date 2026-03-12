using AlfaCharge.Api.DTO;
using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<LocationController> _logger;

    public LocationController(ApplicationDbContext db, ILogger<LocationController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Get paged list of locations.</summary>
    [HttpGet("Locations")]
    public async Task<ActionResult<PagedResultDto<LocationListDto>>> GetLocations(
        [FromQuery] PagingQueryDto query, CancellationToken ct)
    {
        var q = _db.Locations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(l =>
                (l.LocationName != null && l.LocationName.ToLower().Contains(s)) ||
                (l.City != null && l.City.ToLower().Contains(s)) ||
                l.LocationId.ToLower().Contains(s));
        }

        q = query.SortBy?.ToLowerInvariant() switch
        {
            "locationname" => query.SortDescending ? q.OrderByDescending(l => l.LocationName) : q.OrderBy(l => l.LocationName),
            "city" => query.SortDescending ? q.OrderByDescending(l => l.City) : q.OrderBy(l => l.City),
            "country" => query.SortDescending ? q.OrderByDescending(l => l.Country) : q.OrderBy(l => l.Country),
            _ => q.OrderBy(l => l.LocationName)
        };

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var locations = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var locationIds = locations.Select(l => l.LocationId).ToList();

        var stationCounts = (await _db.ChargePoints
            .Where(cp => cp.LocationId != null && locationIds.Contains(cp.LocationId!))
            .GroupBy(cp => cp.LocationId!)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .ToDictionary(g => g.Key, g => g.Count);

        var connectorRows = await (
            from cp in _db.ChargePoints
            join c in _db.Connectors on cp.Id equals c.ChargePointDbId
            where cp.LocationId != null && locationIds.Contains(cp.LocationId!)
            select new { LocationId = cp.LocationId!, c.Status })
            .ToListAsync(ct);

        var connectorStats = connectorRows
            .GroupBy(x => x.LocationId)
            .ToDictionary(g => g.Key, g => (
                Available: g.Count(x => x.Status == ConnectorStatus.Available),
                Charging: g.Count(x => x.Status == ConnectorStatus.Charging)));

        var items = locations.Select(l => new LocationListDto
        {
            Id = l.Id,
            LocationId = l.LocationId,
            LocationName = l.LocationName,
            City = l.City,
            Country = l.Country,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            BusinessName = l.BusinessName,
            StationCount = stationCounts.GetValueOrDefault(l.LocationId, 0),
            AvailableConnectors = connectorStats.TryGetValue(l.LocationId, out var cs) ? cs.Available : 0,
            ChargingConnectors = connectorStats.TryGetValue(l.LocationId, out var cs2) ? cs2.Charging : 0
        }).ToList();

        return Ok(new PagedResultDto<LocationListDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    /// <summary>Get all locations as a flat list (for dropdowns).</summary>
    [HttpGet("all")]
    public async Task<ActionResult<List<LocationListDto>>> GetAllLocations(CancellationToken ct)
    {
        var locations = await _db.Locations
            .OrderBy(l => l.LocationName)
            .ToListAsync(ct);

        var stationCounts = (await _db.ChargePoints
            .Where(cp => cp.LocationId != null)
            .GroupBy(cp => cp.LocationId!)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct))
            .ToDictionary(g => g.Key, g => g.Count);

        return Ok(locations.Select(l => new LocationListDto
        {
            Id = l.Id,
            LocationId = l.LocationId,
            LocationName = l.LocationName,
            City = l.City,
            Country = l.Country,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            BusinessName = l.BusinessName,
            StationCount = stationCounts.GetValueOrDefault(l.LocationId, 0)
        }).ToList());
    }

    /// <summary>Get a single location by its string ID.</summary>
    [HttpGet("{locationId}")]
    public async Task<ActionResult<LocationListDto>> GetLocation(string locationId, CancellationToken ct)
    {
        var location = await _db.Locations
            .FirstOrDefaultAsync(l => l.LocationId == locationId, ct);

        if (location is null) return NotFound();

        var stationCount = await _db.ChargePoints.CountAsync(cp => cp.LocationId == locationId, ct);

        var connectorStatuses = await (
            from cp in _db.ChargePoints
            join c in _db.Connectors on cp.Id equals c.ChargePointDbId
            where cp.LocationId == locationId
            select c.Status)
            .ToListAsync(ct);

        return Ok(new LocationListDto
        {
            Id = location.Id,
            LocationId = location.LocationId,
            LocationName = location.LocationName,
            City = location.City,
            Country = location.Country,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            BusinessName = location.BusinessName,
            StationCount = stationCount,
            AvailableConnectors = connectorStatuses.Count(s => s == ConnectorStatus.Available),
            ChargingConnectors = connectorStatuses.Count(s => s == ConnectorStatus.Charging)
        });
    }

    /// <summary>Create a new location.</summary>
    [HttpPost("AddLocation")]
    public async Task<ActionResult<LocationListDto>> AddLocation(
        [FromBody] LocationUpsertDto dto, CancellationToken ct)
    {
        if (await _db.Locations.AnyAsync(l => l.LocationId == dto.LocationId, ct))
            return Conflict($"Location '{dto.LocationId}' already exists.");

        var location = new AlfaCharge.Domain.Entities.Location
        {
            Id = Guid.NewGuid(),
            LocationId = dto.LocationId,
            LocationName = dto.LocationName,
            City = dto.City,
            Country = dto.Country,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            BusinessName = dto.BusinessName ?? string.Empty,
            BusinessOwner = dto.BusinessOwner
        };

        _db.Locations.Add(location);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetLocation), new { locationId = location.LocationId },
            new LocationListDto
            {
                Id = location.Id,
                LocationId = location.LocationId,
                LocationName = location.LocationName,
                City = location.City,
                Country = location.Country,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                BusinessName = location.BusinessName,
                StationCount = 0
            });
    }

    /// <summary>Update an existing location.</summary>
    [HttpPut("UpdateLocation")]
    public async Task<IActionResult> UpdateLocation(
        [FromQuery] string id, [FromBody] LocationUpsertDto dto, CancellationToken ct)
    {
        var location = await _db.Locations.FirstOrDefaultAsync(l => l.LocationId == id, ct);
        if (location is null) return NotFound();

        location.LocationId = dto.LocationId;
        location.LocationName = dto.LocationName;
        location.City = dto.City;
        location.Country = dto.Country;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.BusinessName = dto.BusinessName ?? string.Empty;
        location.BusinessOwner = dto.BusinessOwner;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Delete a location by its string ID.</summary>
    [HttpDelete("{locationId}")]
    public async Task<IActionResult> DeleteLocation(string locationId, CancellationToken ct)
    {
        var location = await _db.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId, ct);
        if (location is null) return NotFound();

        _db.Locations.Remove(location);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}