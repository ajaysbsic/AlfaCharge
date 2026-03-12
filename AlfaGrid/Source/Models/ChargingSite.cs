namespace AlfaGrid.Source.Models
{
    public class ChargingSite
    {
        public string Id { get; init; } = Guid.NewGuid().ToString("N");
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string Address { get; init; } = string.Empty;
        public double DistanceKm { get; set; }
        public double Rating { get; init; } = 0;
        public string[] ConnectorTypes { get; init; } = Array.Empty<string>();
    }
}