namespace AlfaCharge.Api.DTO;

/// <summary>
/// Dashboard metrics DTO.
/// </summary>
public sealed class DashboardMetricsDto
{
    public int ActiveStations { get; set; }
    public int ChargingSessionsToday { get; set; }
    public double KwhDeliveredToday { get; set; }
    public int FaultedStations { get; set; }
    public int PendingFirmwareUpdates { get; set; }
}

/// <summary>
/// Chart data point DTO.
/// </summary>
public sealed class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}

/// <summary>
/// Sessions chart data DTO (last 24 hours, hourly).
/// </summary>
public sealed class SessionsChartDto
{
    public List<ChartDataPointDto> Data { get; set; } = [];
}

/// <summary>
/// Energy chart data DTO (last 7 days, daily).
/// </summary>
public sealed class EnergyChartDto
{
    public List<ChartDataPointDto> Data { get; set; } = [];
}

/// <summary>
/// OCPP traffic chart data DTO.
/// </summary>
public sealed class OcppTrafficChartDto
{
    public List<ChartDataPointDto> InboundData { get; set; } = [];
    public List<ChartDataPointDto> OutboundData { get; set; } = [];
}

/// <summary>
/// Error statistics chart DTO.
/// </summary>
public sealed class ErrorsChartDto
{
    public List<ChartDataPointDto> Data { get; set; } = [];
}

/// <summary>
/// Error statistics DTO (legacy, for internal use).
/// </summary>
public sealed class ErrorStatsDto
{
    public string ErrorCode { get; set; } = string.Empty;
    public int Count { get; set; }
}
