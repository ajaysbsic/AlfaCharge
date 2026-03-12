using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities;

/// <summary>
/// Represents an RFID card for EV charging authorization.
/// </summary>
public class RfidCard
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The RFID tag identifier (idTag in OCPP terms).
    /// </summary>
    [Required, MaxLength(64)]
    public string IdTag { get; set; } = default!;

    /// <summary>
    /// Optional user ID this card is assigned to.
    /// </summary>
    [MaxLength(128)]
    public string? UserId { get; set; }

    /// <summary>
    /// Status: Active, Blocked, Expired.
    /// </summary>
    [MaxLength(32)]
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Optional expiry date for the card.
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// Last time this card was used for authorization.
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>
    /// Station where the card was last used.
    /// </summary>
    [MaxLength(128)]
    public string? LastUsedStationId { get; set; }

    /// <summary>
    /// Last transaction ID associated with this card.
    /// </summary>
    public Guid? LastTransactionId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
