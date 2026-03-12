using System.Net.Http.Json;
using System.Text.Json;
using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// HTTP client wrapper with standardized error handling and cancellation support.
/// </summary>
public sealed class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Execute a GET request and return the result.
    /// </summary>
    public async Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, ct);
            return await HandleResponse<T>(response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("GET request to {Url} was cancelled", url);
            return ApiResult<T>.Failure("Request was cancelled");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during GET to {Url}", url);
            return ApiResult<T>.Failure($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during GET to {Url}", url);
            return ApiResult<T>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Execute a POST request with a body and return the result.
    /// </summary>
    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url, 
        TRequest body, 
        CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, body, _jsonOptions, ct);
            return await HandleResponse<TResponse>(response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("POST request to {Url} was cancelled", url);
            return ApiResult<TResponse>.Failure("Request was cancelled");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during POST to {Url}", url);
            return ApiResult<TResponse>.Failure($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during POST to {Url}", url);
            return ApiResult<TResponse>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Execute a POST request without expecting a response body.
    /// </summary>
    public async Task<ApiResult> PostAsync<TRequest>(string url, TRequest body, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, body, _jsonOptions, ct);
            return await HandleResponse(response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("POST request to {Url} was cancelled", url);
            return ApiResult.Failure("Request was cancelled");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during POST to {Url}", url);
            return ApiResult.Failure($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during POST to {Url}", url);
            return ApiResult.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Execute a PUT request.
    /// </summary>
    public async Task<ApiResult> PutAsync<TRequest>(string url, TRequest body, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(url, body, _jsonOptions, ct);
            return await HandleResponse(response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("PUT request to {Url} was cancelled", url);
            return ApiResult.Failure("Request was cancelled");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during PUT to {Url}", url);
            return ApiResult.Failure($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during PUT to {Url}", url);
            return ApiResult.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Execute a DELETE request.
    /// </summary>
    public async Task<ApiResult> DeleteAsync(string url, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(url, ct);
            return await HandleResponse(response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("DELETE request to {Url} was cancelled", url);
            return ApiResult.Failure("Request was cancelled");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during DELETE to {Url}", url);
            return ApiResult.Failure($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during DELETE to {Url}", url);
            return ApiResult.Failure($"Unexpected error: {ex.Message}");
        }
    }

    private async Task<ApiResult<T>> HandleResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, ct);
            return data is not null 
                ? ApiResult<T>.Success(data) 
                : ApiResult<T>.Failure("Empty response", (int)response.StatusCode);
        }

        var errorContent = await response.Content.ReadAsStringAsync(ct);
        _logger.LogWarning("API error {StatusCode}: {Error}", (int)response.StatusCode, errorContent);
        return ApiResult<T>.Failure(errorContent, (int)response.StatusCode);
    }

    private async Task<ApiResult> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return ApiResult.Success();
        }

        var errorContent = await response.Content.ReadAsStringAsync(ct);
        _logger.LogWarning("API error {StatusCode}: {Error}", (int)response.StatusCode, errorContent);
        return ApiResult.Failure(errorContent, (int)response.StatusCode);
    }
}
