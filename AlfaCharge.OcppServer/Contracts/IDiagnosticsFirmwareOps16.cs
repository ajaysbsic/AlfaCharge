using AlfaCharge.OcppServer.Contracts.DTO.Messages;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IDiagnosticsFirmwareOps16
    {
        // CSMS -> CP
        Task<GetDiagnosticsConf16> GetDiagnosticsAsync(string chargePointId, GetDiagnosticsReq16 req, CancellationToken ct);
        Task<UpdateFirmwareConf16> UpdateFirmwareAsync(string chargePointId, UpdateFirmwareReq16 req, CancellationToken ct);

        //CP -> CSMS Incoming notifications from CP:
        Task HandleDiagnosticsStatusNotificationAsync(string chargePointId, DiagnosticsStatusNotificationReq16 req, CancellationToken ct);
        Task HandleFirmwareStatusNotificationAsync(string chargePointId, FirmwareStatusNotificationReq16 req, CancellationToken ct);
    }
}