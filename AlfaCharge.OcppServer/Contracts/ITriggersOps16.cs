namespace AlfaCharge.OcppServer.Contracts
{
    // TriggerMessage for OCPP 1.6
    public interface ITriggersOps16
    {
        Task<string> TriggerMessageAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
