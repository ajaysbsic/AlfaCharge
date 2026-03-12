namespace AlfaCharge.Admin.Models;

/// <summary>
/// Paged result for server-side pagination.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// Paging request parameters.
/// </summary>
public class PagingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

/// <summary>
/// Filter parameters for stations.
/// </summary>
public sealed class StationFilter
{
    public string? LocationId { get; set; }
    public string? Status { get; set; }
    public string? FirmwareVersion { get; set; }
}
