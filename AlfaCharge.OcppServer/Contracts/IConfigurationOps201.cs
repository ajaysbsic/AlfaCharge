using AlfaCharge.OcppServer.Contracts.DTO.Message201;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IConfigurationOps201
    {
        // CSMS -> CP
        Task<GetVariablesConf201> GetVariablesAsync(string chargePointId, GetVariablesReq201 req, CancellationToken ct);
        Task<SetVariablesConf201> SetVariablesAsync(string chargePointId, SetVariablesReq201 req, CancellationToken ct);

        // Inventory reporting
        Task<GetBaseReportConf201> GetBaseReportAsync(string chargePointId, GetBaseReportReq201 req, CancellationToken ct);

        // CP -> CSMS (inventory/DM data stream) //Incoming reports (notify report chunks):
        Task HandleNotifyReportAsync(string chargePointId, NotifyReportReq201 req, CancellationToken ct);
    }
}