namespace AlfaCharge.OcppServer.Contracts
{
    // Reservation operations OCPP 2.0.1/2.1
    public interface IReservationOps201
    {
        Task<string> ReserveNowAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> CancelReservationAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
