using AlfaCharge.Domain.Entities;
using AlfaCharge.Domain.Models;

namespace AlfaCharge.Infrastructure.DB.Contracts
{
    public interface IOCPPServices
    {
        Task<string> SendOCPPCommand(string chargePointId, string command, object? payload = null);

        Task<string> GetOCPPStatus(string chargePointId);

        Task<string> StartOCPPTransaction(string chargePointId, int connectorId, string idTag);

        Task<string> StopOCPPTransaction(string chargePointId, int transactionId);

        Task<List<OCPPLog>> GetOCPPMessageLogs(string? chargePointId = null, DateTime? from = null, DateTime? to = null);

        Task<bool> ClearOCPPMessageLogs(string? chargePointId = null, DateTime? from = null, DateTime? to = null);

        Task<string> GetOCPPConfiguration(string chargePointId, List<string> keys);

        Task<string> ChangeOCPPConfiguration(string chargePointId, Dictionary<string, string> configurations);

        Task<string> UpdateFirmware(string chargePointId, string firmwareUrl);

        Task<string> GetOCPPDiagnostics(string chargePointId);

        Task<string> ClearCache(string chargePointId);

        Task<string> GetOCPPStatus(string chargePointId, DateTime? at);

        Task<string> SendOCPPCommand(string chargePointId, string command, object? payload, bool urgent, DateTime? at);

        Task<int> GetActiveTransactionId(string chargePointId, int connectorId, DateTime? at);

        Task<string> SendRawOCPPMessage(string chargePointId, string rawMessage, DateTime? at);

        Task<string> UnlockConnector(string chargePointId, int connectorId, DateTime? at);

        Task<string> ResetOCPPStation(string chargePointId, string resetType, DateTime? at);

        //Task<int> GetActiveTransactionId(string chargePointId, int connectorId);
        //Task<string> SendRawOCPPMessage(string chargePointId, string rawMessage);
        //Task<string> StartOCPPTransaction(string chargePointId, int connectorId, string idTag, DateTime? at);
        //Task<string> StopOCPPTransaction(string chargePointId, int transactionId, DateTime? at);
        //Task<string> GetOCPPDiagnostics(string chargePointId, string location);
        //Task<string> ClearCache(string chargePointId, bool force, DateTime? before);
        //Task<string> UnlockConnector(string chargePointId, int connectorId);
        //Task<string> GetOCPPDiagnostics(string chargePointId, string location, bool retry);
        //Task<string> ResetOCPPStation(string chargePointId, string resetType);
        //Task<string> GetOCPPStatus(string chargePointId, bool detailed);
        //Task<string> GetOCPPStatus(string chargePointId, bool detailed, DateTime? at);
        //Task<string> SendOCPPCommand(string chargePointId, string command, object? payload, bool urgent);
    }
}
