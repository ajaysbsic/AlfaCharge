using System.Collections.Generic;

namespace AlfaGrid.Source.Models
{
    public class ChargingLocation
    {
        public string TenantId { get; set; }
        public int BusinessId { get; set; }
        public int LocationPk { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Stations { get; set; }
        public int Connectors { get; set; }
        public decimal MaxPowerSupply { get; set; }
        public int SiteRating { get; set; }
        public bool IsReservable { get; set; }
        public bool IsPublicViewAllowed { get; set; }
        public bool IsPublic { get; set; }

        // Location coordinates for navigation
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Location information
        public string OperatingHours { get; set; } = "Open 24hrs";
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }

        // Navigation property for stations
        public List<ChargingStation> StationsList { get; set; } = new List<ChargingStation>();

        // Computed property for UI display
        public List<ConnectorGroup> ConnectorGroups { get; set; } = new List<ConnectorGroup>();

        // Facilities available at this location
        public List<Facility> Facilities { get; set; } = new List<Facility>();

        public string AvailabilityStatus
        {
            get
            {
                var availableCount = ConnectorGroups.Sum(g => g.AvailableConnectors);
                var totalCount = ConnectorGroups.Sum(g => g.TotalConnectors);
                return $"{availableCount} of {totalCount} Available";
            }
        }
    }
}
