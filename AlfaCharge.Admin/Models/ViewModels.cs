namespace AlfaCharge.Admin.Models;

/// <summary>
/// Station view model for list display.
/// </summary>
public class StationViewModel
{
    public Guid Id { get; set; }
    public string ChargePointId { get; set; } = string.Empty;
    public string? StationName { get; set; }
    public string? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Status { get; set; }
    public string? Model { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? SerialNumber { get; set; }
    public DateTimeOffset? LastHeartbeat { get; set; }
    public int ConnectorCount { get; set; }
    public bool IsConnected { get; set; }
}

/// <summary>
/// Station detail view model.
/// </summary>
public sealed class StationDetailViewModel : StationViewModel
{
    public List<ConnectorViewModel> Connectors { get; set; } = [];
    public List<RecentActivityViewModel> RecentActivity { get; set; } = [];
}

/// <summary>
/// Connector view model.
/// </summary>
public sealed class ConnectorViewModel
{
    public Guid Id { get; set; }
    public int ConnectorNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public DateTimeOffset? LastStatusTimestamp { get; set; }
    public string? PowerType { get; set; }
    public double PowerKw { get; set; }
}

/// <summary>
/// Recent activity item for station detail.
/// </summary>
public sealed class RecentActivityViewModel
{
    public DateTimeOffset Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Location view model.
/// </summary>
public sealed class LocationViewModel
{
    public Guid Id { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string? LocationName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? BusinessName { get; set; }
    public int StationCount { get; set; }
    public int AvailableConnectors { get; set; }
    public int ChargingConnectors { get; set; }
}

/// <summary>
/// User view model.
/// </summary>
public sealed class UserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}

/// <summary>
/// RFID card view model.
/// </summary>
public sealed class RfidCardViewModel
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
/// OCPP log view model.
/// </summary>
public sealed class OcppLogViewModel
{
    public Guid Id { get; set; }
    public string ChargePointId { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public int MessageTypeId { get; set; }
    public string? MessageId { get; set; }
    public string? Action { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public string? ResultCode { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string TruncatedPayload => PayloadJson.Length > 100 
        ? PayloadJson[..100] + "..." 
        : PayloadJson;
}

/// <summary>
/// Live session view model.
/// </summary>
public sealed class LiveSessionViewModel
{
    public string ChargePointId { get; set; } = string.Empty;
    public int? TransactionId { get; set; }
    public string? IdTag { get; set; }
    public int? ConnectorId { get; set; }
    public string Status { get; set; } = string.Empty;
    public double EnergyKwh { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public TimeSpan Elapsed => DateTimeOffset.UtcNow - StartTime;
    public string? FirmwareVersion { get; set; }
}
