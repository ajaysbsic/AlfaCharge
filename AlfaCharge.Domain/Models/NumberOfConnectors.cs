namespace AlfaCharge.Domain.Models
{
    public class NumberOfConnectors
    {
        public int Id { get; set; }
        public int Available { get; set; }
        public int Charging { get; set; }
        public int Unavailable { get; set; }
        public int Total { get; set; }
    }
}
