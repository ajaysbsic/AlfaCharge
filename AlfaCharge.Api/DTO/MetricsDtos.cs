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
/// Sessions chart data DTO.
/// </summary>
public sealed class SessionsChartDto
{
    public List<ChartDataPointDto> HourlyData { get; set; } = [];
    public List<ChartDataPointDto> DailyData { get; set; } = [];
}

/// <summary>
/// Energy chart data DTO.
/// </summary>
public sealed class EnergyChartDto
{
    public List<ChartDataPointDto> DailyData { get; set; } = [];
    public List<ChartDataPointDto> WeeklyData { get; set; } = [];
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
/// Error statistics DTO.
/// </summary>
public sealed class ErrorStatsDto
{
    public string ErrorCode { get; set; } = string.Empty;
    public int Count { get; set; }
}
