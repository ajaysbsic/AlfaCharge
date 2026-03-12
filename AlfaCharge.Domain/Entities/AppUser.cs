using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities;

/// <summary>
/// Represents an application user (Admin or CPO operator).
/// </summary>
public class AppUser
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(256)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(256)]
    public string Email { get; set; } = default!;

    [MaxLength(512)]
    public string? PasswordHash { get; set; }

    /// <summary>
    /// User role: Admin, CPO.
    /// </summary>
    [Required, MaxLength(64)]
    public string Role { get; set; } = "CPO";

    /// <summary>
    /// Whether the user account is locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Locations this user has access to (for CPO users).
    /// Stored as comma-separated location IDs.
    /// </summary>
    [MaxLength(2048)]
    public string? AssignedLocationIds { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
}
