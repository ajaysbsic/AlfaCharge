namespace AlfaCharge.OcppServer.Contracts
{
    // Reservation operations OCPP 1.6
    public interface IReservationOps16
    {
        Task<string> ReserveNowAsync(string chargePointId, object payload, CancellationToken ct);
        Task<string> CancelReservationAsync(string chargePointId, object payload, CancellationToken ct);
    }
}
