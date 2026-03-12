namespace AlfaCharge.OcppServer.Contracts
{
    // Local authorization list operations OCPP 1.6
    public interface ILocalAuthListOps16
    {
        Task<string> GetLocalListVersionAsync(string chargePointId, CancellationToken ct);
        Task<string> SendLocalListAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
