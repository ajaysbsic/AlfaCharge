using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Api.DTO;

/// <summary>
/// OCPP action result DTO.
/// </summary>
public sealed class OcppActionResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Response { get; set; }
    public string? ErrorCode { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Remote start transaction request DTO.
/// </summary>
public sealed class RemoteStartDto
{
    public int? ConnectorId { get; set; }

    [MaxLength(64)]
    public string? IdTag { get; set; }
    
    public int? EvseId { get; set; }
    
    public string? RemoteStartId { get; set; }
    
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Remote stop transaction request DTO.
/// </summary>
public sealed class RemoteStopDto
{
    public int TransactionId { get; set; }
    
    public int? TransactionId16 { get; set; }
    
    public string? TransactionId201 { get; set; }
    
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Reset request DTO.
/// </summary>
public sealed class ResetDto
{
    [Required]
    public string Type { get; set; } = "Soft";
}

/// <summary>
/// Get diagnostics request DTO.
/// </summary>
public sealed class GetDiagnosticsDto
{
    [Required]
    public string Location { get; set; } = string.Empty;
    public int? Retries { get; set; }
    public int? RetryInterval { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
}

/// <summary>
/// Update firmware request DTO.
/// </summary>
public sealed class UpdateFirmwareDto
{
    [Required]
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset RetrieveDate { get; set; } = DateTimeOffset.UtcNow;
    public int? Retries { get; set; }
    public int? RetryInterval { get; set; }
}

/// <summary>
/// Trigger message request DTO.
/// </summary>
public sealed class TriggerMessageDto
{
    [Required]
    public string RequestedMessage { get; set; } = string.Empty;
    public int? ConnectorId { get; set; }
}

/// <summary>
/// Reserve now request DTO.
/// </summary>
public sealed class ReserveNowDto
{
    [Required]
    public int ConnectorId { get; set; }
    [Required]
    public DateTimeOffset ExpiryDate { get; set; }
    [Required, MaxLength(64)]
    public string IdTag { get; set; } = string.Empty;
    [Required]
    public int ReservationId { get; set; }
}

/// <summary>
/// Cancel reservation request DTO.
/// </summary>
public sealed class CancelReservationDto
{
    [Required]
    public int ReservationId { get; set; }
}

/// <summary>
/// Send local list request DTO.
/// </summary>
public sealed class SendLocalListDto
{
    [Required]
    public int ListVersion { get; set; }
    [Required]
    public string UpdateType { get; set; } = "Full";
    public List<LocalAuthEntryDto> LocalAuthorizationList { get; set; } = [];
}

/// <summary>
/// Local authorization entry DTO.
/// </summary>
public sealed class LocalAuthEntryDto
{
    [Required, MaxLength(64)]
    public string IdTag { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
}

/// <summary>
/// Clear charging profile request DTO.
/// </summary>
public sealed class ClearChargingProfileDto
{
    public int? Id { get; set; }
    public int? ConnectorId { get; set; }
    public string? ChargingProfilePurpose { get; set; }
    public int? StackLevel { get; set; }
}

/// <summary>
/// Set charging profile request DTO.
/// </summary>
public sealed class SetChargingProfileDto
{
    [Required]
    public int ConnectorId { get; set; }
    [Required]
    public ChargingProfileDto CsChargingProfiles { get; set; } = new();
}

/// <summary>
/// Charging profile DTO.
/// </summary>
public sealed class ChargingProfileDto
{
    public int ChargingProfileId { get; set; }
    public int StackLevel { get; set; }
    public string ChargingProfilePurpose { get; set; } = "TxDefaultProfile";
    public string ChargingProfileKind { get; set; } = "Relative";
}

/// <summary>
/// Get composite schedule request DTO.
/// </summary>
public sealed class GetCompositeScheduleDto
{
    [Required]
    public int ConnectorId { get; set; }
    [Required]
    public int Duration { get; set; }
    public string? ChargingRateUnit { get; set; }
}
