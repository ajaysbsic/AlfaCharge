using AlfaCharge.Api.DTO;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Services;

public sealed class MetricsQueryService : IMetricsQueryService
{
    private readonly ApplicationDbContext _db;
    private readonly OcppConnectionManager _connectionManager;

    public MetricsQueryService(ApplicationDbContext db, OcppConnectionManager connectionManager)
    {
        _db = db;
        _connectionManager = connectionManager;
    }

    public async Task<DashboardMetricsDto> GetDashboardMetricsAsync(CancellationToken ct)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var activeStations = _connectionManager.GetConnectedCount();

        var sessionsToday = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= today && t.StartedAt < tomorrow)
            .CountAsync(ct);

        var kwhToday = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= today && t.StartedAt < tomorrow && t.KWh.HasValue)
            .SumAsync(t => t.KWh ?? 0, ct);

        var faultedStations = await _db.Connector
            .Where(c => c.Status == AlfaCharge.Domain.Models.ConnectorStatus.Faulted)
            .Select(c => c.ChargePointDbId)
            .Distinct()
            .CountAsync(ct);

        var pendingFirmware = await _db.OcppJobs
            .Where(j => (j.JobType == AlfaCharge.Domain.Models.OcppJobType.FirmwareUpdate16 || j.JobType == AlfaCharge.Domain.Models.OcppJobType.FirmwareUpdate201)
                     && j.Status == AlfaCharge.Domain.Models.OcppJobStatus.Created)
            .CountAsync(ct);

        return new DashboardMetricsDto
        {
            ActiveStations = activeStations,
            ChargingSessionsToday = sessionsToday,
            KwhDeliveredToday = kwhToday,
            FaultedStations = faultedStations,
            PendingFirmwareUpdates = pendingFirmware
        };
    }

    public async Task<SessionsChartDto> GetSessionsChartAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var last24Hours = now.AddHours(-24);
        var last7Days = now.AddDays(-7);

        var hourlyData = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= last24Hours)
            .GroupBy(t => t.StartedAt.Hour)
            .Select(g => new ChartDataPointDto
            {
                Label = g.Key.ToString("00") + ":00",
                Value = g.Count()
            })
            .OrderBy(d => d.Label)
            .ToListAsync(ct);

        var dailyData = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= last7Days)
            .GroupBy(t => t.StartedAt.Date)
            .Select(g => new ChartDataPointDto
            {
                Label = g.Key.ToString("MMM dd"),
                Value = g.Count(),
                Timestamp = g.Key
            })
            .OrderBy(d => d.Timestamp)
            .ToListAsync(ct);

        return new SessionsChartDto
        {
            HourlyData = hourlyData,
            DailyData = dailyData
        };
    }

    public async Task<EnergyChartDto> GetEnergyChartAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var last7Days = now.AddDays(-7);
        var last4Weeks = now.AddDays(-28);

        var dailyData = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= last7Days && t.KWh.HasValue)
            .GroupBy(t => t.StartedAt.Date)
            .Select(g => new ChartDataPointDto
            {
                Label = g.Key.ToString("MMM dd"),
                Value = g.Sum(t => t.KWh ?? 0),
                Timestamp = g.Key
            })
            .OrderBy(d => d.Timestamp)
            .ToListAsync(ct);

        var weeklyTransactions = await _db.ChargingTransactions
            .Where(t => t.StartedAt >= last4Weeks && t.KWh.HasValue)
            .ToListAsync(ct);

        var weeklyData = weeklyTransactions
            .GroupBy(t => System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                t.StartedAt.DateTime,
                System.Globalization.CalendarWeekRule.FirstDay,
                DayOfWeek.Monday))
            .Select(g => new ChartDataPointDto
            {
                Label = $"Week {g.Key}",
                Value = g.Sum(t => t.KWh ?? 0)
            })
            .OrderBy(d => d.Label)
            .ToList();

        return new EnergyChartDto
        {
            DailyData = dailyData,
            WeeklyData = weeklyData
        };
    }

    public async Task<OcppTrafficChartDto> GetOcppTrafficChartAsync(CancellationToken ct)
    {
        var last24Hours = DateTimeOffset.UtcNow.AddHours(-24);

        var trafficData = await _db.OcppLogs
            .Where(l => l.Timestamp >= last24Hours)
            .GroupBy(l => new { l.Timestamp.Hour, l.Direction })
            .Select(g => new
            {
                Hour = g.Key.Hour,
                Direction = g.Key.Direction,
                Count = g.Count()
            })
            .ToListAsync(ct);

        var inboundData = trafficData
            .Where(t => t.Direction == "inbound")
            .Select(t => new ChartDataPointDto
            {
                Label = t.Hour.ToString("00") + ":00",
                Value = t.Count
            })
            .OrderBy(d => d.Label)
            .ToList();

        var outboundData = trafficData
            .Where(t => t.Direction == "outbound")
            .Select(t => new ChartDataPointDto
            {
                Label = t.Hour.ToString("00") + ":00",
                Value = t.Count
            })
            .OrderBy(d => d.Label)
            .ToList();

        return new OcppTrafficChartDto
        {
            InboundData = inboundData,
            OutboundData = outboundData
        };
    }

    public async Task<List<ErrorStatsDto>> GetErrorStatsAsync(CancellationToken ct)
    {
        var last7Days = DateTimeOffset.UtcNow.AddDays(-7);

        return await _db.Connector
            .Where(c => c.ErrorCode != null && c.LastStatusTimestamp >= last7Days)
            .GroupBy(c => c.ErrorCode)
            .Select(g => new ErrorStatsDto
            {
                ErrorCode = g.Key ?? "Unknown",
                Count = g.Count()
            })
            .OrderByDescending(e => e.Count)
            .Take(10)
            .ToListAsync(ct);
    }
}
