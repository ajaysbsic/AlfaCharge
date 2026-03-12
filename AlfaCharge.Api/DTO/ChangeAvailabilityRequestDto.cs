namespace AlfaCharge.Api.DTO
{
    public sealed class ChangeAvailabilityRequestDto
    {
        // 1.6: use 'type' + 'connectorId'
        public string? Type { get; set; }
        public int? ConnectorId { get; set; }

        // 2.0.1: use 'operationalStatus' + optional 'evseId' and/or 'connectorId'
        public string? OperationalStatus { get; set; }
        public int? EvseId { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }
}