namespace AlfaCharge.OcppServer.Contracts
{
    // Charging profile operations OCPP 1.6
    public interface IChargingProfileOps16
    {
        Task<string> ClearChargingProfileAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> SetChargingProfileAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> GetCompositeScheduleAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
