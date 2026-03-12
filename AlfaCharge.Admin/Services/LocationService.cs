using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Service for location CRUD operations.
/// </summary>
public sealed class LocationService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<LocationService> _logger;

    public LocationService(ApiClient apiClient, ILogger<LocationService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get paged list of locations.
    /// </summary>
    public async Task<ApiResult<PagedResult<LocationViewModel>>> GetLocationsAsync(
        PagingRequest paging,
        CancellationToken ct = default)
    {
        var url = BuildUrl("/api/location/Locations", paging);
        return await _apiClient.GetAsync<PagedResult<LocationViewModel>>(url, ct);
    }

    /// <summary>
    /// Get all locations for dropdowns.
    /// </summary>
    public async Task<ApiResult<List<LocationViewModel>>> GetAllLocationsAsync(CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<List<LocationViewModel>>("/api/location/Locations", ct);
    }

    /// <summary>
    /// Get location by ID.
    /// </summary>
    public async Task<ApiResult<LocationViewModel>> GetLocationAsync(
        string locationId, 
        CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<LocationViewModel>($"/api/location/{locationId}", ct);
    }

    /// <summary>
    /// Create a new location.
    /// </summary>
    public async Task<ApiResult<LocationViewModel>> CreateLocationAsync(
        LocationEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync<LocationEditModel, LocationViewModel>(
            "/api/location/AddLocation", model, ct);
    }

    /// <summary>
    /// Update a location.
    /// </summary>
    public async Task<ApiResult> UpdateLocationAsync(
        string locationId, 
        LocationEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PutAsync($"/api/location/UpdateLocation?id={locationId}", model, ct);
    }

    /// <summary>
    /// Delete a location.
    /// </summary>
    public async Task<ApiResult> DeleteLocationAsync(string locationId, CancellationToken ct = default)
    {
        return await _apiClient.DeleteAsync($"/api/location/{locationId}", ct);
    }

    private static string BuildUrl(string baseUrl, PagingRequest paging)
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

        return $"{baseUrl}?{string.Join("&", queryParams)}";
    }
}

/// <summary>
/// Model for location create/edit form.
/// </summary>
public sealed class LocationEditModel
{
    public string LocationId { get; set; } = string.Empty;
    public string? LocationName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? BusinessName { get; set; }
    public string? BusinessOwner { get; set; }
}
