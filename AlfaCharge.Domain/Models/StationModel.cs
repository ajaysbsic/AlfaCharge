namespace AlfaCharge.Domain.Models
{
    public class StationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Manufacturer { get; set; }
        public string ManufacturerName { get; set; }

        public string MaxElectricPower { get; set; }

    }
}
