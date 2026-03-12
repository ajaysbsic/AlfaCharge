namespace AlfaCharge.OcppServer.Contracts.DTO
{
    /// <summary>
    /// In OCPP 2.0.1, StatusNotification is used for availability (Operative/Inoperative) per the basic availability set.
    /// For broader events, NotifyEvent is used (not implemented here).
    /// </summary>
    public class Ocpp201StatusNotificationRequest
    {
        public string Status { get; set; } = default!;      // Operative | Inoperative
        public int? EvseId { get; set; }                    // optional
        public int? ConnectorId { get; set; }               // optional
        public DateTimeOffset? Timestamp { get; set; }
    }
    public class Ocpp201StatusNotificationResponse { }
}