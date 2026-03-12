namespace AlfaCharge.Admin.Models;

/// <summary>
/// Standard API result wrapper for error handling.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed class ApiResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }
    public int? StatusCode { get; private init; }

    private ApiResult() { }

    public static ApiResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static ApiResult<T> Failure(string error, int? statusCode = null) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}

/// <summary>
/// Non-generic API result for void operations.
/// </summary>
public sealed class ApiResult
{
    public bool IsSuccess { get; private init; }
    public string? Error { get; private init; }
    public int? StatusCode { get; private init; }

    private ApiResult() { }

    public static ApiResult Success() => new() { IsSuccess = true };

    public static ApiResult Failure(string error, int? statusCode = null) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}
