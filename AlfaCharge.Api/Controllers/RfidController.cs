using AlfaCharge.Api.DTO;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers;

/// <summary>
/// Admin API for RFID card management.
/// </summary>
[Route("api/admin/rfid")]
[ApiController]
public class RfidController : ControllerBase
{
    private readonly ILogger<RfidController> _logger;
    private readonly ApplicationDbContext _db;

    public RfidController(ILogger<RfidController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Get paged list of RFID cards.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RfidCardListDto>>> GetRfidCards(
        [FromQuery] PagingQueryDto query,
        CancellationToken ct)
    {
        try
        {
            var dbQuery = _db.RfidCards.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                dbQuery = dbQuery.Where(r => r.IdTag.ToLower().Contains(search));
            }

            var totalCount = await dbQuery.CountAsync(ct);

            dbQuery = query.SortBy?.ToLower() switch
            {
                "idtag" => query.SortDescending ? dbQuery.OrderByDescending(r => r.IdTag) : dbQuery.OrderBy(r => r.IdTag),
                "status" => query.SortDescending ? dbQuery.OrderByDescending(r => r.Status) : dbQuery.OrderBy(r => r.Status),
                "lastused" => query.SortDescending ? dbQuery.OrderByDescending(r => r.LastUsedAt) : dbQuery.OrderBy(r => r.LastUsedAt),
                _ => dbQuery.OrderBy(r => r.IdTag)
            };

            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new RfidCardListDto
                {
                    Id = r.Id,
                    IdTag = r.IdTag,
                    UserId = r.UserId,
                    Status = r.Status,
                    ExpiryDate = r.ExpiryDate,
                    LastUsedAt = r.LastUsedAt,
                    LastUsedStationId = r.LastUsedStationId
                })
                .ToListAsync(ct);

            // Get user names for cards with assigned users
            var userIds = items.Where(i => i.UserId != null).Select(i => i.UserId).ToList();
            var users = await _db.AppUsers
                .Where(u => userIds.Contains(u.Id.ToString()))
                .ToDictionaryAsync(u => u.Id.ToString(), u => u.Name, ct);

            foreach (var item in items)
            {
                if (item.UserId != null && users.TryGetValue(item.UserId, out var userName))
                {
                    item.UserName = userName;
                }
            }

            return Ok(new PagedResultDto<RfidCardListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RFID cards");
            return StatusCode(500, "Error retrieving RFID cards");
        }
    }

    /// <summary>
    /// Get RFID card by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RfidCardListDto>> GetRfidCard(Guid id, CancellationToken ct)
    {
        var card = await _db.RfidCards.FindAsync([id], ct);
        if (card is null)
            return NotFound();

        string? userName = null;
        if (card.UserId != null && Guid.TryParse(card.UserId, out var userId))
        {
            var user = await _db.AppUsers.FindAsync([userId], ct);
            userName = user?.Name;
        }

        return Ok(new RfidCardListDto
        {
            Id = card.Id,
            IdTag = card.IdTag,
            UserId = card.UserId,
            UserName = userName,
            Status = card.Status,
            ExpiryDate = card.ExpiryDate,
            LastUsedAt = card.LastUsedAt,
            LastUsedStationId = card.LastUsedStationId
        });
    }

    /// <summary>
    /// Create a new RFID card.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RfidCardListDto>> CreateRfidCard([FromBody] RfidCardUpsertDto dto, CancellationToken ct)
    {
        if (await _db.RfidCards.AnyAsync(r => r.IdTag == dto.IdTag, ct))
            return Conflict("RFID card with this IdTag already exists");

        var card = new RfidCard
        {
            Id = Guid.NewGuid(),
            IdTag = dto.IdTag,
            UserId = dto.UserId?.ToString(),
            Status = dto.Status,
            ExpiryDate = dto.ExpiryDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.RfidCards.Add(card);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetRfidCard), new { id = card.Id }, new RfidCardListDto
        {
            Id = card.Id,
            IdTag = card.IdTag,
            UserId = card.UserId,
            Status = card.Status,
            ExpiryDate = card.ExpiryDate
        });
    }

    /// <summary>
    /// Update an RFID card.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateRfidCard(Guid id, [FromBody] RfidCardUpsertDto dto, CancellationToken ct)
    {
        var card = await _db.RfidCards.FindAsync([id], ct);
        if (card is null)
            return NotFound();

        card.IdTag = dto.IdTag;
        card.UserId = dto.UserId?.ToString();
        card.Status = dto.Status;
        card.ExpiryDate = dto.ExpiryDate;
        card.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Delete an RFID card.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteRfidCard(Guid id, CancellationToken ct)
    {
        var card = await _db.RfidCards.FindAsync([id], ct);
        if (card is null)
            return NotFound();

        _db.RfidCards.Remove(card);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
