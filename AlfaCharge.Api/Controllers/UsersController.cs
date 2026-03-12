using AlfaCharge.Api.DTO;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Controllers;

/// <summary>
/// Admin API for user management.
/// </summary>
[Route("api/admin/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _db;

    public UsersController(ILogger<UsersController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Get paged list of users.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserListDto>>> GetUsers(
        [FromQuery] PagingQueryDto query,
        CancellationToken ct)
    {
        try
        {
            var dbQuery = _db.AppUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                dbQuery = dbQuery.Where(u =>
                    u.Name.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var totalCount = await dbQuery.CountAsync(ct);

            dbQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending ? dbQuery.OrderByDescending(u => u.Name) : dbQuery.OrderBy(u => u.Name),
                "email" => query.SortDescending ? dbQuery.OrderByDescending(u => u.Email) : dbQuery.OrderBy(u => u.Email),
                "role" => query.SortDescending ? dbQuery.OrderByDescending(u => u.Role) : dbQuery.OrderBy(u => u.Role),
                _ => dbQuery.OrderBy(u => u.Name)
            };

            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsLocked = u.IsLocked,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync(ct);

            return Ok(new PagedResultDto<UserListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Error retrieving users");
        }
    }

    /// <summary>
    /// Get user by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserListDto>> GetUser(Guid id, CancellationToken ct)
    {
        var user = await _db.AppUsers.FindAsync([id], ct);
        if (user is null)
            return NotFound();

        return Ok(new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsLocked = user.IsLocked,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserListDto>> CreateUser([FromBody] UserCreateDto dto, CancellationToken ct)
    {
        if (await _db.AppUsers.AnyAsync(u => u.Email == dto.Email, ct))
            return Conflict("User with this email already exists");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            AssignedLocationIds = dto.AssignedLocationIds,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Update a user.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto dto, CancellationToken ct)
    {
        var user = await _db.AppUsers.FindAsync([id], ct);
        if (user is null)
            return NotFound();

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.AssignedLocationIds = dto.AssignedLocationIds;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        var user = await _db.AppUsers.FindAsync([id], ct);
        if (user is null)
            return NotFound();

        _db.AppUsers.Remove(user);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Reset user password.
    /// </summary>
    [HttpPost("{id:guid}/reset-password")]
    public async Task<ActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        var user = await _db.AppUsers.FindAsync([id], ct);
        if (user is null)
            return NotFound();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Lock or unlock a user.
    /// </summary>
    [HttpPost("{id:guid}/lock")]
    public async Task<ActionResult> LockUser(Guid id, [FromBody] LockUserDto dto, CancellationToken ct)
    {
        var user = await _db.AppUsers.FindAsync([id], ct);
        if (user is null)
            return NotFound();

        user.IsLocked = dto.IsLocked;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
