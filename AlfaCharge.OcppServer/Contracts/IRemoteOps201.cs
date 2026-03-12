namespace AlfaCharge.OcppServer.Contracts
{
    // Remote operations for OCPP 2.0.1/2.1
    // Note: OCPP 2.x uses different action names than 1.6
    public interface IRemoteOps201
    {
        // RequestStartTransaction (not RemoteStartTransaction in 2.x)
        Task<string> RequestStartTransactionAsync(string chargePointId, object payload, CancellationToken ct);
        
        // RequestStopTransaction (not RemoteStopTransaction in 2.x)
        Task<string> RequestStopTransactionAsync(string chargePointId, object payload, CancellationToken ct);
        
        // Reset (same name as 1.6)
        Task<string> ResetAsync(string chargePointId, object payload, CancellationToken ct);
        
        // GetLog (replaces GetDiagnostics in 2.x)
        Task<string> GetLogAsync(string chargePointId, object payload, CancellationToken ct);
        
        // UpdateFirmware (enhanced in 2.x)
        Task<string> UpdateFirmwareAsync(string chargePointId, object payload, CancellationToken ct);
        
        // TriggerMessage (same concept, different structure in 2.x)
        Task<string> TriggerMessageAsync(string chargePointId, object payload, CancellationToken ct);
        
        // UnlockConnector (same as 1.6)
        Task<string> UnlockConnectorAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
