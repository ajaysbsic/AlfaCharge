using System.Text.Json.Serialization;

namespace AlfaCharge.Domain.Models
{
    public enum ChargePointStatus
    {
        Available,
        Charging,
        Unavailable,
        Faulted,
        Online,
        Offline
    }

    public enum SecurityProtocol
    {
        [JsonPropertyName("TLS 1.2")]
        TLS12,

        [JsonPropertyName("TLS 1.3")]
        TLS13,

        SSL,
        None
    }

    public enum ConnectorStatus
    {
        Available,
        Charging,
        Unavailable,
        Faulted
    }

    public enum ConnectorType
    {
        [JsonPropertyName("AC Type 2")]
        ACType2,

        [JsonPropertyName("DC CCS")]
        DCCCS,

        CHAdeMO
    }

    public enum NotificationStatus
    {
        Accepted,
        Rejected,
        Pending
    }

    public enum  IdTagStatus
    {
        Accepted,
        Blocked,
        Expired
    }

    public enum OcppJobType 
    {
        FirmwareUpdate16, 
        Diagnostics16, 
        Log201, 
        FirmwareUpdate201 
    }

    public enum OcppJobStatus
    {
        Created, 
        Accepted,
        Running, 
        Succeeded,
        Failed,
        Rejected,
        Canceled
    }
}