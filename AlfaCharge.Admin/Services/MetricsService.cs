using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Service for dashboard metrics and charts.
/// </summary>
public sealed class MetricsService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService(ApiClient apiClient, ILogger<MetricsService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard KPI metrics.
    /// </summary>
    public async Task<ApiResult<DashboardMetrics>> GetDashboardMetricsAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<DashboardMetrics>("/api/metrics/dashboard", ct);
    }

    /// <summary>
    /// Get sessions chart data.
    /// </summary>
    public async Task<ApiResult<SessionsChartData>> GetSessionsChartAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<SessionsChartData>("/api/metrics/sessions", ct);
    }

    /// <summary>
    /// Get energy consumption chart data.
    /// </summary>
    public async Task<ApiResult<EnergyChartData>> GetEnergyChartAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<EnergyChartData>("/api/metrics/energy", ct);
    }

    /// <summary>
    /// Get OCPP traffic chart data.
    /// </summary>
    public async Task<ApiResult<OcppTrafficChartData>> GetOcppTrafficChartAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<OcppTrafficChartData>("/api/metrics/ocpp-traffic", ct);
    }

    /// <summary>
    /// Get top errors statistics.
    /// </summary>
    public async Task<ApiResult<ErrorsChartData>> GetErrorsChartAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<ErrorsChartData>("/api/metrics/errors", ct);
    }
}
