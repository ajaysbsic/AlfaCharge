using AlfaCharge.Api.DTO;

namespace AlfaCharge.Api.Services;

public interface IMetricsQueryService
{
    Task<DashboardMetricsDto> GetDashboardMetricsAsync(CancellationToken ct);
    Task<SessionsChartDto> GetSessionsChartAsync(CancellationToken ct);
    Task<EnergyChartDto> GetEnergyChartAsync(CancellationToken ct);
    Task<OcppTrafficChartDto> GetOcppTrafficChartAsync(CancellationToken ct);
    Task<ErrorsChartDto> GetErrorStatsAsync(CancellationToken ct);
}
