namespace AlfaCharge.Admin.Models;

/// <summary>
/// Result of an OCPP action execution.
/// </summary>
public sealed class OcppActionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Response { get; set; }
    public string? ErrorCode { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Remote start transaction request.
/// </summary>
public sealed class RemoteStartRequest
{
    public int ConnectorId { get; set; }
    public string IdTag { get; set; } = string.Empty;
}

/// <summary>
/// Remote stop transaction request.
/// </summary>
public sealed class RemoteStopRequest
{
    public int TransactionId { get; set; }
}

/// <summary>
/// Reset request types.
/// </summary>
public enum ResetType
{
    Soft,
    Hard
}

/// <summary>
/// Reset request.
/// </summary>
public sealed class ResetRequest
{
    public ResetType Type { get; set; } = ResetType.Soft;
}

/// <summary>
/// Get diagnostics request.
/// </summary>
public sealed class GetDiagnosticsRequest
{
    public string Location { get; set; } = string.Empty;
    public int? Retries { get; set; }
    public int? RetryInterval { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? StopTime { get; set; }
}

/// <summary>
/// Update firmware request.
/// </summary>
public sealed class UpdateFirmwareRequest
{
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset RetrieveDate { get; set; } = DateTimeOffset.UtcNow;
    public int? Retries { get; set; }
    public int? RetryInterval { get; set; }
}

/// <summary>
/// Trigger message request.
/// </summary>
public sealed class TriggerMessageRequest
{
    public string RequestedMessage { get; set; } = string.Empty;
    public int? ConnectorId { get; set; }
}

/// <summary>
/// Reserve now request.
/// </summary>
public sealed class ReserveNowRequest
{
    public int ConnectorId { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public string IdTag { get; set; } = string.Empty;
    public int ReservationId { get; set; }
}

/// <summary>
/// Cancel reservation request.
/// </summary>
public sealed class CancelReservationRequest
{
    public int ReservationId { get; set; }
}

/// <summary>
/// Send local list request.
/// </summary>
public sealed class SendLocalListRequest
{
    public int ListVersion { get; set; }
    public string UpdateType { get; set; } = "Full";
    public List<LocalAuthorizationEntry> LocalAuthorizationList { get; set; } = [];
}

/// <summary>
/// Local authorization entry.
/// </summary>
public sealed class LocalAuthorizationEntry
{
    public string IdTag { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
}

/// <summary>
/// Clear charging profile request.
/// </summary>
public sealed class ClearChargingProfileRequest
{
    public int? Id { get; set; }
    public int? ConnectorId { get; set; }
    public string? ChargingProfilePurpose { get; set; }
    public int? StackLevel { get; set; }
}

/// <summary>
/// Set charging profile request.
/// </summary>
public sealed class SetChargingProfileRequest
{
    public int ConnectorId { get; set; }
    public ChargingProfile CsChargingProfiles { get; set; } = new();
}

/// <summary>
/// Charging profile definition.
/// </summary>
public sealed class ChargingProfile
{
    public int ChargingProfileId { get; set; }
    public int StackLevel { get; set; }
    public string ChargingProfilePurpose { get; set; } = "TxDefaultProfile";
    public string ChargingProfileKind { get; set; } = "Relative";
}

/// <summary>
/// Get composite schedule request.
/// </summary>
public sealed class GetCompositeScheduleRequest
{
    public int ConnectorId { get; set; }
    public int Duration { get; set; }
    public string? ChargingRateUnit { get; set; }
}
