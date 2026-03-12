using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities
{
    public class TransactionMeterSample
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; } // FK to ChargingTransaction.Id
        public DateTimeOffset Timestamp { get; set; }

        // e.g. "Energy.Active.Import.Register", "Power.Active.Import", "Current.Import"…
        public string Measurand { get; set; } = "Energy.Active.Import.Register";

        // e.g. "Wh", "kWh", "W", "A", "V"…
        public string Unit { get; set; } = "Wh";

        // stored as string per OCPP; parse to decimal/double as needed
        public string Value { get; set; } = "0";
    }
}