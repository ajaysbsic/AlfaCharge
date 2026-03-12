using AlfaCharge.Domain.Entities;
using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AlfaCharge.Infrastructure.DB.Services
{
    public class OCPPServices : IOCPPServices
    {
        private readonly ApplicationDbContext _ocppDbContext;
        public OCPPServices(ApplicationDbContext db)
        {
            _ocppDbContext = db;
        }

        public async Task<string> ChangeOCPPConfiguration(string chargePointId, Dictionary<string, string> configurations)
        {
            foreach (var pair in configurations)
            {
                var existing = await _ocppDbContext.OcppConfigurations
                    .FirstOrDefaultAsync(x => x.ChargePointId == chargePointId && x.Key == pair.Key);

                if (existing is null)
                {
                    _ocppDbContext.OcppConfigurations.Add(new OcppConfigurationEntry
                    {
                        Id = Guid.NewGuid(),
                        ChargePointId = chargePointId,
                        Key = pair.Key,
                        Value = pair.Value,
                        Readonly = false,
                        UpdatedAt = DateTimeOffset.UtcNow
                    });
                }
                else
                {
                    existing.Value = pair.Value;
                    existing.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            await _ocppDbContext.SaveChangesAsync();
            return "Configuration updated";
        }

        public async Task<string> ClearCache(string chargePointId)
        {
            await CreateAuditLog(chargePointId, "ClearCache", new { });
            return "ClearCache command queued";
        }

        public async Task<bool> ClearOCPPMessageLogs(string? chargePointId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _ocppDbContext.OcppLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(chargePointId))
                query = query.Where(x => x.ChargePointId == chargePointId);

            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            var logs = await query.ToListAsync();
            if (logs.Count == 0)
                return false;

            _ocppDbContext.OcppLogs.RemoveRange(logs);
            await _ocppDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetActiveTransactionId(string chargePointId, int connectorId, DateTime? at)
        {
            var atTime = at ?? DateTime.UtcNow;
            var tx = await _ocppDbContext.ChargingTransactions
                .Where(t => t.ChargePointId == chargePointId
                         && t.State == "Active"
                         && t.StartedAt <= atTime)
                .OrderByDescending(t => t.StartedAt)
                .FirstOrDefaultAsync();

            return tx?.Ocpp16TransactionId ?? 0;
        }

        public async Task<string> GetOCPPConfiguration(string chargePointId, List<string> keys)
        {
            var query = _ocppDbContext.OcppConfigurations.Where(x => x.ChargePointId == chargePointId);
            if (keys.Count > 0)
                query = query.Where(x => keys.Contains(x.Key));

            var result = await query
                .Select(x => new { x.Key, x.Value, x.Readonly, x.UpdatedAt })
                .ToListAsync();

            return JsonSerializer.Serialize(result);
        }

        public async Task<string> GetOCPPDiagnostics(string chargePointId)
        {
            await CreateOcppJob(chargePointId, OcppJobType.Diagnostics16, null);
            return "Diagnostics job queued";
        }

        public async Task<List<OCPPLog>> GetOCPPMessageLogs(string? chargePointId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _ocppDbContext.OcppLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(chargePointId))
                query = query.Where(x => x.ChargePointId == chargePointId);

            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            return await query
                .OrderByDescending(x => x.Timestamp)
                .Take(1000)
                .ToListAsync();
        }

        public async Task<string> GetOCPPStatus(string chargePointId)
        {
            var cp = await _ocppDbContext.ChargePoints.FirstOrDefaultAsync(x => x.ChargePointId == chargePointId);
            if (cp is null)
                return "Unknown";

            return cp.Status ?? "Unknown";
        }

        public async Task<string> GetOCPPStatus(string chargePointId, DateTime? at)
        {
            var statusAt = at ?? DateTime.UtcNow;
            var status = await _ocppDbContext.StatusHistories
                .Where(s => s.ChargePointId == chargePointId && s.OccurredAt <= statusAt)
                .OrderByDescending(s => s.OccurredAt)
                .Select(s => s.Status)
                .FirstOrDefaultAsync();

            return status ?? "Unknown";
        }

        public async Task<string> ResetOCPPStation(string chargePointId, string resetType, DateTime? at)
        {
            await CreateAuditLog(chargePointId, "Reset", new { type = resetType, at });
            return "Reset command queued";
        }

        public Task<string> SendOCPPCommand(string chargePointId, string command, object? payload = null)
        {
            return SendOCPPCommand(chargePointId, command, payload, urgent: false, at: null);
        }

        public async Task<string> SendOCPPCommand(string chargePointId, string command, object? payload, bool urgent, DateTime? at)
        {
            await CreateAuditLog(chargePointId, command, new { payload, urgent, at });
            return $"{command} command queued";
        }

        public async Task<string> SendRawOCPPMessage(string chargePointId, string rawMessage, DateTime? at)
        {
            await CreateAuditLog(chargePointId, "RawMessage", new { rawMessage, at });
            return "Raw message queued";
        }

        public async Task<string> StartOCPPTransaction(string chargePointId, int connectorId, string idTag)
        {
            await CreateAuditLog(chargePointId, "RemoteStartTransaction", new { connectorId, idTag });
            return "Start transaction command queued";
        }

        public async Task<string> StopOCPPTransaction(string chargePointId, int transactionId)
        {
            await CreateAuditLog(chargePointId, "RemoteStopTransaction", new { transactionId });
            return "Stop transaction command queued";
        }

        public async Task<string> UnlockConnector(string chargePointId, int connectorId, DateTime? at)
        {
            await CreateAuditLog(chargePointId, "UnlockConnector", new { connectorId, at });
            return "Unlock connector command queued";
        }

        public async Task<string> UpdateFirmware(string chargePointId, string firmwareUrl)
        {
            await CreateOcppJob(chargePointId, OcppJobType.FirmwareUpdate16, firmwareUrl);
            return "Firmware update job queued";
        }

        private async Task CreateOcppJob(string chargePointId, OcppJobType jobType, string? location)
        {
            _ocppDbContext.OcppJobs.Add(new OcppJob
            {
                Id = Guid.NewGuid(),
                ChargePointId = chargePointId,
                JobType = jobType,
                Location = location,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = OcppJobStatus.Created,
                StatusInfo = "Queued by OCPP service"
            });

            await _ocppDbContext.SaveChangesAsync();
        }

        private async Task CreateAuditLog(string chargePointId, string action, object payload)
        {
            _ocppDbContext.OcppLogs.Add(new OCPPLog
            {
                Id = Guid.NewGuid(),
                ChargePointId = chargePointId,
                Direction = "outbound",
                MessageTypeId = 2,
                Action = action,
                PayloadJson = JsonSerializer.Serialize(payload),
                ResultCode = "queued",
                Timestamp = DateTimeOffset.UtcNow
            });

            await _ocppDbContext.SaveChangesAsync();
        }
    }
}
