using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities
{
    public class ChargingTransaction
    {

        public Guid Id { get; set; }

        // URL identity (CP123…), useful to join with logs/WS
        public string ChargePointId { get; set; } = default!;

        // Optional link to your Connector row (if known)
        public Guid? ConnectorDbId { get; set; }

        // OCPP 1.6 transactionId is int (from CP), OCPP 2.0.1 is string (from CP)
        public int? Ocpp16TransactionId { get; set; }
        public string? Ocpp201TransactionId { get; set; }

        // Identification/authorization context (idTag/token)
        public string? IdTag { get; set; }
        public string? IdToken { get; set; }

        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? StoppedAt { get; set; }

        public long? MeterStart { get; set; } // typically Wh on many devices (verify per vendor)
        public long? MeterStop { get; set; }

        // Derived convenience (if meter is Wh)
        public double? KWh { get; set; }

        public string? StopReason { get; set; }   // 1.6 reasons or 2.0.1 event/trigger reason
        public string State { get; set; } = "Active"; // Active/Ended/Failed/Rejected
    }
}