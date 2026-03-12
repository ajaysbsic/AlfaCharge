namespace AlfaCharge.Admin.Models;

/// <summary>
/// Dashboard KPI metrics summary.
/// </summary>
public sealed class DashboardMetrics
{
    public int ActiveStations { get; set; }
    public int ChargingSessionsToday { get; set; }
    public double KwhDeliveredToday { get; set; }
    public int FaultedStations { get; set; }
    public int PendingFirmwareUpdates { get; set; }
}

/// <summary>
/// Chart data point for time-series data.
/// </summary>
public sealed class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

/// <summary>
/// Sessions per hour/day chart data.
/// </summary>
public sealed class SessionsChartData
{
    public List<ChartDataPoint> Data { get; set; } = [];
}

/// <summary>
/// Energy consumption chart data.
/// </summary>
public sealed class EnergyChartData
{
    public List<ChartDataPoint> Data { get; set; } = [];
}

/// <summary>
/// OCPP traffic chart data.
/// </summary>
public sealed class OcppTrafficChartData
{
    public List<ChartDataPoint> InboundData { get; set; } = [];
    public List<ChartDataPoint> OutboundData { get; set; } = [];
}

/// <summary>
/// Top errors chart data.
/// </summary>
public sealed class ErrorsChartData
{
    public List<ChartDataPoint> Data { get; set; } = [];
}
