using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Api.DTO;

/// <summary>
/// Station list item DTO.
/// </summary>
public class StationListDto
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
/// Station detail DTO.
/// </summary>
public sealed class StationDetailDto : StationListDto
{
    public List<ConnectorDto> Connectors { get; set; } = [];
    public List<RecentActivityDto> RecentActivity { get; set; } = [];
}

/// <summary>
/// Connector DTO.
/// </summary>
public sealed class ConnectorDto
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
/// Recent activity DTO.
/// </summary>
public sealed class RecentActivityDto
{
    public DateTimeOffset Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Station query parameters.
/// </summary>
public sealed class StationQueryDto : PagingQueryDto
{
    public string? LocationId { get; set; }
    public string? Status { get; set; }
    public string? FirmwareVersion { get; set; }
}

/// <summary>
/// Station create/update DTO.
/// </summary>
public sealed class StationUpsertDto
{
    [Required, MaxLength(128)]
    public string ChargePointId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? StationName { get; set; }

    [MaxLength(128)]
    public string? LocationId { get; set; }

    [MaxLength(128)]
    public string? Model { get; set; }

    [MaxLength(128)]
    public string? FirmwareVersion { get; set; }

    [MaxLength(128)]
    public string? SerialNumber { get; set; }

    [Range(1, 10)]
    public int ConnectorCount { get; set; } = 1;
}
