using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AlfaCharge.Domain.Models;

namespace AlfaCharge.Domain.Entities
{
    public class Connector
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ChargePointDbId { get; set; }   // FK to our ChargePoint table's PK (uuid)

        /// <summary>
        /// For OCPP 1.6 this is the Request.connectorId (0=whole CP; 1..N=connector #).
        /// For OCPP 2.0.1 you may also carry evseId+connectorId if needed.
        /// </summary>
        public int ConnectorNumber { get; set; }

        /// <summary>
        /// Runtime status (1.6): available|charging|unavailable|faulted|reserved|finishing|preparing|suspendedEv|suspendedEvse
        /// Map the 1.6 enums into snake/camel as needed by your UI.
        /// </summary>
        [MaxLength(64)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConnectorStatus Status { get; set; }

        /// <summary>
        /// Availability (2.0.1): Operative|Inoperative (we store lowercase for UI simplicity).
        /// </summary>
        [MaxLength(64)]
        public string? OperationalStatus { get; set; }

        [MaxLength(128)]
        public string? ErrorCode { get; set; }

        public DateTimeOffset? LastStatusTimestamp { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConnectorType PowerType { get; set; }

        [JsonPropertyName("power_kw")]
        public double PowerKw { get; set; }
        public Standard Standard { get; set; }
        public string MaxVoltage { get; set; }
        public string MaxAmperage { get; set; }
        public string MaxElectricPower { get; set; }
        public int ConnectorSequence { get; set; }
    }
}