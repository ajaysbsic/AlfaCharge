using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Service for RFID card management operations.
/// </summary>
public sealed class RfidService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<RfidService> _logger;

    public RfidService(ApiClient apiClient, ILogger<RfidService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get paged list of RFID cards.
    /// </summary>
    public async Task<ApiResult<PagedResult<RfidCardViewModel>>> GetRfidCardsAsync(
        PagingRequest paging,
        CancellationToken ct = default)
    {
        var url = BuildUrl("/api/admin/rfid", paging);
        return await _apiClient.GetAsync<PagedResult<RfidCardViewModel>>(url, ct);
    }

    /// <summary>
    /// Get RFID card by ID.
    /// </summary>
    public async Task<ApiResult<RfidCardViewModel>> GetRfidCardAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<RfidCardViewModel>($"/api/admin/rfid/{id}", ct);
    }

    /// <summary>
    /// Create a new RFID card.
    /// </summary>
    public async Task<ApiResult<RfidCardViewModel>> CreateRfidCardAsync(
        RfidCardEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync<RfidCardEditModel, RfidCardViewModel>(
            "/api/admin/rfid", model, ct);
    }

    /// <summary>
    /// Update an RFID card.
    /// </summary>
    public async Task<ApiResult> UpdateRfidCardAsync(
        Guid id, 
        RfidCardEditModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PutAsync($"/api/admin/rfid/{id}", model, ct);
    }

    /// <summary>
    /// Delete an RFID card.
    /// </summary>
    public async Task<ApiResult> DeleteRfidCardAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.DeleteAsync($"/api/admin/rfid/{id}", ct);
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
/// Model for RFID card create/edit form.
/// </summary>
public sealed class RfidCardEditModel
{
    public string IdTag { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTimeOffset? ExpiryDate { get; set; }
}
