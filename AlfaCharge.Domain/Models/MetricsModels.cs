namespace AlfaCharge.Domain.Models;

/// <summary>
/// Dashboard metrics aggregate.
/// </summary>
public class DashboardMetricsDto
{
    public int ActiveStations { get; set; }
    public int ChargingSessionsToday { get; set; }
    public double KwhDeliveredToday { get; set; }
    public int FaultedStations { get; set; }
    public int PendingFirmwareUpdates { get; set; }
}

/// <summary>
/// Time-series data point for charts.
/// </summary>
public class TimeSeriesPoint
{
    public DateTimeOffset Timestamp { get; set; }
    public double Value { get; set; }
    public string? Label { get; set; }
}

/// <summary>
/// Sessions statistics.
/// </summary>
public class SessionsStats
{
    public List<TimeSeriesPoint> HourlyData { get; set; } = [];
    public List<TimeSeriesPoint> DailyData { get; set; } = [];
}

/// <summary>
/// Energy consumption statistics.
/// </summary>
public class EnergyStats
{
    public List<TimeSeriesPoint> DailyData { get; set; } = [];
    public List<TimeSeriesPoint> WeeklyData { get; set; } = [];
}

/// <summary>
/// OCPP traffic statistics.
/// </summary>
public class OcppTrafficStats
{
    public List<TimeSeriesPoint> InboundTraffic { get; set; } = [];
    public List<TimeSeriesPoint> OutboundTraffic { get; set; } = [];
}

/// <summary>
/// Error statistics by type.
/// </summary>
public class ErrorStats
{
    public string ErrorCode { get; set; } = string.Empty;
    public int Count { get; set; }
}
