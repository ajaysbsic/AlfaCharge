using AlfaCharge.Domain.Entities;
using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.OcppServer.Versioned_Handlers.TransactionHandlers
{
    public class Ocpp16TransactionHandler : IOcpp16TransactionHandler
    {
        private readonly ApplicationDbContext _db;

        public Ocpp16TransactionHandler(ApplicationDbContext db) => _db = db;

        public async Task<Ocpp16StartTransactionResponse> HandleStartAsync(string cpId, Ocpp16StartTransactionRequest req)
        {
            var tx = new ChargingTransaction
            {
                Id = Guid.NewGuid(),
                ChargePointId = cpId,
                Ocpp16TransactionId = 0, // will set after we assign/receive (some CPs ignore response ID)
                IdTag = req.IdTag,
                StartedAt = req.Timestamp ?? default,
                MeterStart = req.MeterStart,
                State = "Active"
            };
            _db.ChargingTransactions.Add(tx);
            await _db.SaveChangesAsync();

            // Typically CP expects CSMS to return transactionId (int). We’ll generate a local int (hash).
            var txIdInt = Math.Abs(tx.Id.GetHashCode());
            tx.Ocpp16TransactionId = txIdInt;
            await _db.SaveChangesAsync();

            return new Ocpp16StartTransactionResponse
            {
                TransactionId = txIdInt,
                IdTagInfo = new IdTagInfo { Status = IdTagStatus.Accepted }
            };
        }

        public async Task<Ocpp16StopTransactionResponse> HandleStopAsync(string cpId, Ocpp16StopTransactionRequest req)
        {
            var tx = await _db.ChargingTransactions
                .FirstOrDefaultAsync(t => t.Ocpp16TransactionId == req.TransactionId && t.ChargePointId == cpId);

            if (tx == null)
            {
                // Create a tombstone transaction if missing (rare, but defensive).
                tx = new ChargingTransaction
                {
                    Id = Guid.NewGuid(),
                    ChargePointId = cpId,
                    Ocpp16TransactionId = req.TransactionId,
                    StartedAt = req.Timestamp ?? default, // unknown start; set to stop for lack of better info
                    MeterStart = null,
                    State = "Ended"
                };
                _db.ChargingTransactions.Add(tx);
            }

            tx.MeterStop = req.MeterStop;
            tx.StoppedAt = req.Timestamp;
            tx.StopReason = req.Reason ?? "Local";
            tx.State = "Ended";

            if (tx.MeterStart.HasValue && tx.MeterStop.HasValue)
            {
                // Assuming Wh; compute kWh
                tx.KWh = (tx.MeterStop.Value - tx.MeterStart.Value) / 1000.0;
            }

            await _db.SaveChangesAsync();

            return new Ocpp16StopTransactionResponse
            {
                IdTagInfo = new IdTagInfo { Status = IdTagStatus.Accepted }
            };
        }

        public async Task<Ocpp16MeterValuesResponse> HandleMeterValuesAsync(string cpId, Ocpp16MeterValuesRequest req)
        {
            // Attach samples to active transaction (by transactionId if present; else latest active on that CP)
            ChargingTransaction? tx = null;
            if (req.TransactionId.HasValue)
            {
                tx = await _db.ChargingTransactions
                    .FirstOrDefaultAsync(t => t.Ocpp16TransactionId == req.TransactionId.Value && t.ChargePointId == cpId);
            }
            else
            {
                tx = await _db.ChargingTransactions
                    .Where(t => t.ChargePointId == cpId && t.State == "Active")
                    .OrderByDescending(t => t.StartedAt)
                    .FirstOrDefaultAsync();
            }
            if (tx == null) return new Ocpp16MeterValuesResponse();

            foreach (var mv in req.MeterValue)
            {
                foreach (var sv in mv.SampledValue)
                {
                    _db.TransactionMeterSamples.Add(new TransactionMeterSample
                    {
                        Id = Guid.NewGuid(),
                        TransactionId = tx.Id,
                        Timestamp = mv.Timestamp,
                        Measurand = sv.Measurand ?? "Energy.Active.Import.Register",
                        Unit = sv.Unit ?? "Wh",
                        Value = sv.Value
                    });

                    // If this measurand is the energy register, update a rolling kWh quickly (optional)
                    if ((sv.Measurand ?? "").Equals("Energy.Active.Import.Register", StringComparison.OrdinalIgnoreCase)
                        && long.TryParse(sv.Value, out var vWh))
                    {
                        // Use it as a moving meterStop preview
                        tx.MeterStop = vWh;
                        if (tx.MeterStart.HasValue)
                            tx.KWh = Math.Max(0, (vWh - tx.MeterStart.Value) / 1000.0);
                    }
                }
            }
            await _db.SaveChangesAsync();
            return new Ocpp16MeterValuesResponse();
        }
    }
}