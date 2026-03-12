namespace AlfaCharge.OcppServer.Contracts
{
    // Remote operations for OCPP 1.6: RemoteStartTransaction, RemoteStopTransaction, Reset, GetDiagnostics, UpdateFirmware, UnlockConnector
    public interface IRemoteOps16
    {
        Task<string> RemoteStartTransactionAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> RemoteStopTransactionAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> ResetAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> GetDiagnosticsAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> UpdateFirmwareAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> UnlockConnectorAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
