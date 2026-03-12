using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.OcppServer.Versioned_Handlers.TransactionHandlers
{
    public class Ocpp201TransactionHandler : IOcpp201TransactionHandler
    {
        private readonly ApplicationDbContext _db;

        public Ocpp201TransactionHandler(ApplicationDbContext db) => _db = db;

        public async Task<Ocpp201TransactionEventResponse> HandleEventAsync(string cpId, Ocpp201TransactionEventRequest req)
        {
            var tx = await _db.ChargingTransactions
                .FirstOrDefaultAsync(t => t.Ocpp201TransactionId == req.TransactionInfo.TransactionId && t.ChargePointId == cpId);

            if (req.EventType.Equals("Started", StringComparison.OrdinalIgnoreCase))
            {
                if (tx == null)
                {
                    tx = new ChargingTransaction
                    {
                        Id = Guid.NewGuid(),
                        ChargePointId = cpId,
                        Ocpp201TransactionId = req.TransactionInfo.TransactionId,
                        StartedAt = req.Timestamp,
                        State = "Active"
                    };
                    _db.ChargingTransactions.Add(tx);
                }
            }

            // Attach samples (if any)
            if (tx != null && req.MeterValue?.Count > 0)
            {
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
                            Unit = sv.UnitOfMeasure ?? "Wh",
                            Value = sv.Value
                        });

                        if ((sv.Measurand ?? "").Equals("Energy.Active.Import.Register", StringComparison.OrdinalIgnoreCase)
                            && long.TryParse(sv.Value, out var vWh))
                        {
                            if (!tx.MeterStart.HasValue)
                                tx.MeterStart = vWh; // some stations send the register even at Started

                            tx.MeterStop = vWh;
                            if (tx.MeterStart.HasValue)
                                tx.KWh = Math.Max(0, (vWh - tx.MeterStart.Value) / 1000.0);
                        }
                    }
                }
            }

            if (req.EventType.Equals("Ended", StringComparison.OrdinalIgnoreCase))
            {
                if (tx == null)
                {
                    // Create a tombstone if we missed Started (rare)
                    tx = new ChargingTransaction
                    {
                        Id = Guid.NewGuid(),
                        ChargePointId = cpId,
                        Ocpp201TransactionId = req.TransactionInfo.TransactionId,
                        StartedAt = req.Timestamp,
                    };
                    _db.ChargingTransactions.Add(tx);
                }

                tx.StoppedAt = req.Timestamp;
                tx.StopReason = req.TransactionInfo?.StoppedReason ?? req.TriggerReason;
                tx.State = "Ended";
            }

            await _db.SaveChangesAsync();
            return new Ocpp201TransactionEventResponse();
        }
    }
}