using AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IDiagnosticsFirmwareOps201
    {
        // CSMS -> CP
        Task<GetLogConf201> GetLogAsync(string chargePointId, GetLogReq201 req, CancellationToken ct);
        Task<UpdateFirmwareConf201> UpdateFirmwareAsync(string chargePointId, UpdateFirmwareReq201 req, CancellationToken ct);

        // CP -> CSMS // Incoming status notifications
        Task HandleLogStatusNotificationAsync(string chargePointId, LogStatusNotificationReq201 req, CancellationToken ct);
        Task HandleFirmwareStatusNotificationAsync(string chargePointId, FirmwareStatusNotificationReq201 req, CancellationToken ct);
    }
}