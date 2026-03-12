namespace AlfaCharge.OcppServer.Contracts
{
    // Charging profile operations OCPP 2.0.1/2.1
    // Note: OCPP 2.x has significantly different charging profile management
    public interface IChargingProfileOps201
    {
        // ClearChargingProfile exists in 2.x
        Task<string> ClearChargingProfileAsync(string chargePointId, object payload, CancellationToken ct);
        
        // SetChargingProfile exists in 2.x
        Task<string> SetChargingProfileAsync(string chargePointId, object payload, CancellationToken ct);
        
        // GetChargingProfiles (not GetCompositeSchedule in 2.x)
        Task<string> GetChargingProfilesAsync(string chargePointId, object payload, CancellationToken ct);
        
        // GetCompositeSchedule exists in 2.x
        Task<string> GetCompositeScheduleAsync(string chargePointId, object payload, CancellationToken ct);
        
        // ReportChargingProfiles - response from CP, handled in router
        // ClearedChargingLimit - new in 2.x
        Task<string> ClearedChargingLimitAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
