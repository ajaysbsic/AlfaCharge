namespace AlfaGrid.Source.Models
{
    public class StationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Manufacturer { get; set; }
        public string ManufacturerName { get; set; }
        
        // Changed from decimal to string to match JSON format
        public string MaxElectricPower { get; set; }

        // Helper property for numeric conversion
        public decimal MaxElectricPowerNumeric
        {
            get
            {
                if (decimal.TryParse(MaxElectricPower, out var result))
                    return result;
                return 0;
            }
        }
    }
}
