namespace AlfaGrid.Source.Models
{
    /// <summary>
    /// Represents a group of connectors with the same type and power rating for UI display
    /// </summary>
    public class ConnectorGroup
    {
        public string ConnectorType { get; set; } // "AC" or "DC"
        public string Standard { get; set; } // "Type 2" or "CCS2"
        public string ImageSource { get; set; } // "Type_2.png" or "combo_2_ccs.png"
        public decimal PowerRating { get; set; } // 22, 120, 320, etc.
        public int TotalConnectors { get; set; }
        public int AvailableConnectors { get; set; }

        public string PowerRatingText => $"{PowerRating}kW";
        public string AvailabilityText => $"{AvailableConnectors}/{TotalConnectors} Available";
    }
}
