using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Api.DTO;

/// <summary>
/// User list item DTO.
/// </summary>
public sealed class UserListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}

/// <summary>
/// User create DTO.
/// </summary>
public sealed class UserCreateDto
{
    [Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "CPO";

    /// <summary>
    /// Comma-separated location IDs for CPO users.
    /// </summary>
    public string? AssignedLocationIds { get; set; }
}

/// <summary>
/// User update DTO.
/// </summary>
public sealed class UserUpdateDto
{
    [Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "CPO";

    public string? AssignedLocationIds { get; set; }
}

/// <summary>
/// Reset password DTO.
/// </summary>
public sealed class ResetPasswordDto
{
    [Required, MinLength(8), MaxLength(128)]
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Lock/unlock user DTO.
/// </summary>
public sealed class LockUserDto
{
    public bool IsLocked { get; set; }
}
