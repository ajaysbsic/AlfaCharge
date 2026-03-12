using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Domain.Entities
{
    public class ChargePoint
    {
        [Key]
        public Guid Id { get; set; }

        // This is the human/URL id, e.g., "CP123"
        [Required, MaxLength(128)]
        public string ChargePointId { get; set; } = default!;

        [MaxLength(256)]
        public string? Station_name { get; set; }

        // Foreign key to Location by its string "LocationId" (alt key mapping in DbContext)
        [MaxLength(128)]
        public string? LocationId { get; set; }

        [MaxLength(64)]
        public string? Status { get; set; }

        [MaxLength(128)]
        public string? Model { get; set; }

        [MaxLength(128)]
        public string? FirmwareVersion { get; set; }

        [MaxLength(128)]
        public string? SerialNumber { get; set; }
    }
}