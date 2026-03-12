namespace AlfaCharge.OcppServer.Contracts
{
    // TriggerMessage for OCPP 2.0.1/2.1
    public interface ITriggersOps201
    {
        Task<string> TriggerMessageAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
