using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Client for OCPP log access.
/// </summary>
public sealed class OcppLogClient
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<OcppLogClient> _logger;

    public OcppLogClient(ApiClient apiClient, ILogger<OcppLogClient> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get paged list of OCPP logs with filtering.
    /// </summary>
    public async Task<ApiResult<PagedResult<OcppLogViewModel>>> GetLogsAsync(
        OcppLogQuery query,
        CancellationToken ct = default)
    {
        var url = BuildUrl("/api/ocpp/logs", query);
        return await _apiClient.GetAsync<PagedResult<OcppLogViewModel>>(url, ct);
    }

    /// <summary>
    /// Get single log entry by ID.
    /// </summary>
    public async Task<ApiResult<OcppLogViewModel>> GetLogAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<OcppLogViewModel>($"/api/ocpp/logs/{id}", ct);
    }

    /// <summary>
    /// Get distinct actions for filter dropdown.
    /// </summary>
    public async Task<ApiResult<List<string>>> GetActionsAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<List<string>>("/api/ocpp/logs/actions", ct);
    }

    /// <summary>
    /// Get distinct charge point IDs for filter dropdown.
    /// </summary>
    public async Task<ApiResult<List<string>>> GetChargePointsAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<List<string>>("/api/ocpp/logs/chargepoints", ct);
    }

    private static string BuildUrl(string baseUrl, OcppLogQuery query)
    {
        var queryParams = new List<string>
        {
            $"page={query.Page}",
            $"pageSize={query.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(query.Search))
            queryParams.Add($"search={Uri.EscapeDataString(query.Search)}");

        if (!string.IsNullOrWhiteSpace(query.SortBy))
            queryParams.Add($"sortBy={Uri.EscapeDataString(query.SortBy)}");

        if (query.SortDescending)
            queryParams.Add("sortDescending=true");

        if (!string.IsNullOrWhiteSpace(query.ChargePointId))
            queryParams.Add($"chargePointId={Uri.EscapeDataString(query.ChargePointId)}");

        if (!string.IsNullOrWhiteSpace(query.Action))
            queryParams.Add($"action={Uri.EscapeDataString(query.Action)}");

        if (!string.IsNullOrWhiteSpace(query.Direction))
            queryParams.Add($"direction={Uri.EscapeDataString(query.Direction)}");

        if (query.FromDate.HasValue)
            queryParams.Add($"fromDate={query.FromDate.Value:O}");

        if (query.ToDate.HasValue)
            queryParams.Add($"toDate={query.ToDate.Value:O}");

        return $"{baseUrl}?{string.Join("&", queryParams)}";
    }
}

/// <summary>
/// Query parameters for OCPP logs.
/// </summary>
public sealed class OcppLogQuery : PagingRequest
{
    public string? ChargePointId { get; set; }
    public string? Action { get; set; }
    public string? Direction { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}
