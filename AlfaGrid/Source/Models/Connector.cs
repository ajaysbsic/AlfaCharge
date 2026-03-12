namespace AlfaGrid.Source.Models
{
    public class Connector
    {
        public int ConnectorPk { get; set; }
        public string Id { get; set; }
        public ConnectorStandard Standard { get; set; }
        public PowerType PowerType { get; set; }
        public string MaxVoltage { get; set; }
        public string MaxAmperage { get; set; }
        public string MaxElectricPower { get; set; }
        public int ConnectorSequence { get; set; }
        public DateTime LastUpdated { get; set; }

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
