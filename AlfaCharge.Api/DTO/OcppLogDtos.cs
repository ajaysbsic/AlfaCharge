namespace AlfaCharge.Api.DTO;

/// <summary>
/// OCPP log list item DTO.
/// </summary>
public sealed class OcppLogListDto
{
    public Guid Id { get; set; }
    public string ChargePointId { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public int MessageTypeId { get; set; }
    public string? MessageId { get; set; }
    public string? Action { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public string? ResultCode { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

/// <summary>
/// OCPP log query parameters.
/// </summary>
public sealed class OcppLogQueryDto : PagingQueryDto
{
    public string? ChargePointId { get; set; }
    public string? Action { get; set; }
    public string? Direction { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
}
