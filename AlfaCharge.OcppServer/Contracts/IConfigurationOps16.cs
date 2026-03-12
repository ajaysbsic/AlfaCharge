using AlfaCharge.OcppServer.Contracts.DTO.Messages;

namespace AlfaCharge.OcppServer.Contracts
{
    public interface IConfigurationOps16
    {
        // CSMS -> CP
        Task<GetConfigurationConf16> GetConfigurationAsync(string chargePointId, string[]? keys, CancellationToken ct);
        Task<ChangeConfigurationConf16> ChangeConfigurationAsync(string chargePointId, string key, string value, CancellationToken ct);
    }
}