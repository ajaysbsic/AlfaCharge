using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Api.DTO;

/// <summary>
/// RFID card list item DTO.
/// </summary>
public sealed class RfidCardListDto
{
    public Guid Id { get; set; }
    public string IdTag { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Status { get; set; } = "Active";
    public DateTimeOffset? ExpiryDate { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public string? LastUsedStationId { get; set; }
}

/// <summary>
/// RFID card create/update DTO.
/// </summary>
public sealed class RfidCardUpsertDto
{
    [Required, MaxLength(64)]
    public string IdTag { get; set; } = string.Empty;

    public Guid? UserId { get; set; }

    [Required, MaxLength(32)]
    public string Status { get; set; } = "Active";

    public DateTimeOffset? ExpiryDate { get; set; }
}
