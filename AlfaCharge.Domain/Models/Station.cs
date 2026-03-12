using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AlfaCharge.Domain.Entities;

namespace AlfaCharge.Domain.Models
{

    public class Station
    {
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("station_name")]
        public string StationName { get; set; }

        [Required]
        [JsonPropertyName("charge_point_id")]
        public string ChargePointId { get; set; }

        [JsonPropertyName("location_id")]
        public string LocationId { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChargePointStatus Status { get; set; } = ChargePointStatus.Offline;

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("max_power")]
        public double MaxPower { get; set; } = 22;

        [JsonPropertyName("security_protocol")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SecurityProtocol SecurityProtocol { get; set; } = SecurityProtocol.TLS13;

        [JsonPropertyName("qr_code_url")]
        public string QrCodeUrl { get; set; }

        [JsonPropertyName("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("last_online")]
        public DateTime? LastOnline { get; set; }

        [JsonPropertyName("connectors")]
        public List<Connector> Connectors { get; set; }
    }
}