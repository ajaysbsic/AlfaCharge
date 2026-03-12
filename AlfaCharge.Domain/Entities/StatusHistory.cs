using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities
{

    public class StatusHistory
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Human/URL identity of the charge point (e.g., "CP123"), taken from the WS path.
        /// </summary>
        [MaxLength(128)]
        public string ChargePointId { get; set; } = default!;

        /// <summary>
        /// Optional FK to Connector table (when we know which connector/EVSE this relates to).
        /// </summary>
        public Guid? ConnectorDbId { get; set; }

        /// <summary>
        /// Category of the status entry:
        ///  - "runtime"      (OCPP 1.6 StatusNotification — connector runtime state)
        ///  - "availability" (OCPP 2.0.1 StatusNotification — Operative/Inoperative)
        ///  - future: "fault"/"event" (e.g., 2.0.1 NotifyEvent)
        /// </summary>
        [MaxLength(64)]
        public string StatusType { get; set; } = "runtime";

        /// <summary>
        /// The actual status value stored as string to support both:
        ///  - 1.6 runtime enum values via ConnectorStatus.ToString() (e.g., "Available", "Charging", "Faulted", "Unavailable")
        ///  - 2.0.1 availability strings (e.g., "operative", "inoperative")
        /// </summary>
        [MaxLength(128)]
        public string Status { get; set; } = default!;

        /// <summary>
        /// Optional error/fault code (mainly for 1.6).
        /// </summary>
        [MaxLength(256)]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// When this status occurred on the device (or when received).
        /// </summary>
        public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Original payload fragment for diagnostics/audit.
        /// </summary>
        public string? DetailsJson { get; set; }
    }
}