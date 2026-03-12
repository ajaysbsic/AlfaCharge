using AlfaCharge.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Service for station CRUD operations.
/// </summary>
public sealed class StationService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<StationService> _logger;

    public StationService(ApiClient apiClient, ILogger<StationService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get paged list of stations with optional filtering.
    /// </summary>
    public async Task<ApiResult<PagedResult<StationViewModel>>> GetStationsAsync(
        PagingRequest paging,
        StationFilter? filter = null,
        CancellationToken ct = default)
    {
        var url = BuildUrl("/api/admin/stations", paging, filter);
        return await _apiClient.GetAsync<PagedResult<StationViewModel>>(url, ct);
    }

    /// <summary>
    /// Get station by ID.
    /// </summary>
    public async Task<ApiResult<StationDetailViewModel>> GetStationAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<StationDetailViewModel>($"/api/admin/stations/{id}", ct);
    }

    /// <summary>
    /// Create a new station.
    /// </summary>
    public async Task<ApiResult<StationViewModel>> CreateStationAsync(
        StationEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync<StationEditModel, StationViewModel>(
            "/api/admin/stations", model, ct);
    }

    /// <summary>
    /// Update a station.
    /// </summary>
    public async Task<ApiResult> UpdateStationAsync(
        Guid id, 
        StationEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PutAsync($"/api/admin/stations/{id}", model, ct);
    }

    /// <summary>
    /// Delete a station.
    /// </summary>
    public async Task<ApiResult> DeleteStationAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.DeleteAsync($"/api/admin/stations/{id}", ct);
    }

    private static string BuildUrl(string baseUrl, PagingRequest paging, StationFilter? filter)
    {
        var queryParams = new List<string>
        {
            $"page={paging.Page}",
            $"pageSize={paging.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(paging.Search))
            queryParams.Add($"search={Uri.EscapeDataString(paging.Search)}");

        if (!string.IsNullOrWhiteSpace(paging.SortBy))
            queryParams.Add($"sortBy={Uri.EscapeDataString(paging.SortBy)}");

        if (paging.SortDescending)
            queryParams.Add("sortDescending=true");

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.LocationId))
                queryParams.Add($"locationId={Uri.EscapeDataString(filter.LocationId)}");

            if (!string.IsNullOrWhiteSpace(filter.Status))
                queryParams.Add($"status={Uri.EscapeDataString(filter.Status)}");

            if (!string.IsNullOrWhiteSpace(filter.FirmwareVersion))
                queryParams.Add($"firmwareVersion={Uri.EscapeDataString(filter.FirmwareVersion)}");
        }

        return $"{baseUrl}?{string.Join("&", queryParams)}";
    }
}

/// <summary>
/// Model for station create/edit form.
/// </summary>
public sealed class StationEditModel
{
    [Required]
    public string ChargePointId { get; set; } = string.Empty;
    public string? StationName { get; set; }
    public string? LocationId { get; set; }
    public string? Model { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? SerialNumber { get; set; }

    [Range(1, 10)]
    public int ConnectorCount { get; set; } = 1;
}
