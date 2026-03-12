namespace AlfaCharge.OcppServer.Contracts
{
    // Local authorization list operations OCPP 2.0.1/2.1
    // Note: In OCPP 2.x, authorization list management is handled differently
    public interface ILocalAuthListOps201
    {
        // GetLocalListVersion is not in OCPP 2.0.1/2.1
        // Instead, use GetLocalAuthorizationListVersion or check via variable monitoring
        // For now, keeping interface for compatibility but may not be directly supported
        Task<string> GetLocalListVersionAsync(string chargePointId, CancellationToken ct);
        
        // SendLocalList exists in 2.x
        Task<string> SendLocalListAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
