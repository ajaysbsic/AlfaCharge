using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Service for user management operations.
/// </summary>
public sealed class UserService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<UserService> _logger;

    public UserService(ApiClient apiClient, ILogger<UserService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Get paged list of users.
    /// </summary>
    public async Task<ApiResult<PagedResult<UserViewModel>>> GetUsersAsync(
        PagingRequest paging,
        CancellationToken ct = default)
    {
        var url = BuildUrl("/api/admin/users", paging);
        return await _apiClient.GetAsync<PagedResult<UserViewModel>>(url, ct);
    }

    /// <summary>
    /// Get user by ID.
    /// </summary>
    public async Task<ApiResult<UserViewModel>> GetUserAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<UserViewModel>($"/api/admin/users/{id}", ct);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    public async Task<ApiResult<UserViewModel>> CreateUserAsync(
        UserCreateModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync<UserCreateModel, UserViewModel>(
            "/api/admin/users", model, ct);
    }

    /// <summary>
    /// Update a user.
    /// </summary>
    public async Task<ApiResult> UpdateUserAsync(
        Guid id, 
        UserUpdateModel model, 
        CancellationToken ct = default)
    {
        return await _apiClient.PutAsync($"/api/admin/users/{id}", model, ct);
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    public async Task<ApiResult> DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        return await _apiClient.DeleteAsync($"/api/admin/users/{id}", ct);
    }

    /// <summary>
    /// Reset user password.
    /// </summary>
    public async Task<ApiResult> ResetPasswordAsync(
        Guid id, 
        string newPassword, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync($"/api/admin/users/{id}/reset-password", 
            new { NewPassword = newPassword }, ct);
    }

    /// <summary>
    /// Lock or unlock a user.
    /// </summary>
    public async Task<ApiResult> SetLockedAsync(
        Guid id, 
        bool isLocked, 
        CancellationToken ct = default)
    {
        return await _apiClient.PostAsync($"/api/admin/users/{id}/lock", 
            new { IsLocked = isLocked }, ct);
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
/// Model for user creation.
/// </summary>
public sealed class UserCreateModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "CPO";
    public string? AssignedLocationIds { get; set; }
}

/// <summary>
/// Model for user update.
/// </summary>
public sealed class UserUpdateModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "CPO";
    public string? AssignedLocationIds { get; set; }
}
