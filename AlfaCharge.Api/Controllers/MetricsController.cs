using AlfaCharge.Api.DTO;
using AlfaCharge.Api.Services;
using AlfaCharge.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlfaCharge.Api.Controllers;

/// <summary>
/// API for dashboard metrics and charts.
/// </summary>
[Route("api/metrics")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly ILogger<MetricsController> _logger;
    private readonly IMetricsQueryService _metricsQuery;

    public MetricsController(
        ILogger<MetricsController> logger,
        IMetricsQueryService metricsQuery)
    {
        _logger = logger;
        _metricsQuery = metricsQuery;
    }

    /// <summary>
    /// Get dashboard KPI metrics.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DTO.DashboardMetricsDto>> GetDashboardMetrics(CancellationToken ct)
    {
        try
        {
            return Ok(await _metricsQuery.GetDashboardMetricsAsync(ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard metrics");
            return StatusCode(500, "Error retrieving dashboard metrics");
        }
    }

    /// <summary>
    /// Get sessions chart data.
    /// </summary>
    [HttpGet("sessions")]
    public async Task<ActionResult<SessionsChartDto>> GetSessionsChart(CancellationToken ct)
    {
        try
        {
            return Ok(await _metricsQuery.GetSessionsChartAsync(ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions chart");
            return StatusCode(500, "Error retrieving sessions chart");
        }
    }

    /// <summary>
    /// Get energy consumption chart data.
    /// </summary>
    [HttpGet("energy")]
    public async Task<ActionResult<EnergyChartDto>> GetEnergyChart(CancellationToken ct)
    {
        try
        {
            return Ok(await _metricsQuery.GetEnergyChartAsync(ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting energy chart");
            return StatusCode(500, "Error retrieving energy chart");
        }
    }

    /// <summary>
    /// Get OCPP traffic chart data.
    /// </summary>
    [HttpGet("ocpp-traffic")]
    public async Task<ActionResult<OcppTrafficChartDto>> GetOcppTrafficChart(CancellationToken ct)
    {
        try
        {
            return Ok(await _metricsQuery.GetOcppTrafficChartAsync(ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OCPP traffic chart");
            return StatusCode(500, "Error retrieving OCPP traffic chart");
        }
    }

    /// <summary>
    /// Get top errors statistics.
    /// </summary>
    [HttpGet("errors")]
    public async Task<ActionResult<List<ErrorStatsDto>>> GetErrorStats(CancellationToken ct)
    {
        try
        {
            return Ok(await _metricsQuery.GetErrorStatsAsync(ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting error stats");
            return StatusCode(500, "Error retrieving error statistics");
        }
    }
}
