namespace AlfaCharge.Domain.Models
{
    public class StationOverviewData
    {
        public int Id { get; set; }
        public List<Station> Data { get; set; } = new();
        public int Count { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}