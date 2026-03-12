using System.Collections.Generic;

namespace AlfaGrid.Source.Models
{
    public class ChargingStation
    {
        public string LocationName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string LocationUid { get; set; }
        public int LocationPk { get; set; }
        public int EvsePk { get; set; }
        public string Uid { get; set; }
        public string ChargePointId { get; set; }
        public string Name { get; set; }
        public string InstallationStatus { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public StationModel Model { get; set; }
        public List<Connector> Connectors { get; set; } = new List<Connector>();
        public string Firmware { get; set; }
        public string EvseId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
