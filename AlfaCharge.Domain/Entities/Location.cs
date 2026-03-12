using System.ComponentModel.DataAnnotations;
using AlfaCharge.Domain.Models;

namespace AlfaCharge.Domain.Entities
{
    public  class Location
    {
        [Key]
        public Guid Id { get; set; }

        // Human-readable/slug id (string) to match your earlier design
        [Required, MaxLength(128)]
        public string LocationId { get; set; } = default!;

        [MaxLength(256)]
        public string? LocationName { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(128)]
        public string? City { get; set; }

        [MaxLength(128)]
        public string? Country { get; set; }

        [MaxLength(256)]
        public string? BusinessOwner { get; set; }

        public ICollection<ChargePoint> ChargePoints { get; set; } = new List<ChargePoint>();

        public int NumberOfEvses { get; set; }

        public NumberOfConnectors NumberOfConnectors { get; set; } = new();
        public string BusinessName { get; set; } = string.Empty;
    }
}
