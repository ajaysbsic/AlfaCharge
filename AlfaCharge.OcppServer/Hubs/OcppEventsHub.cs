using Microsoft.AspNetCore.SignalR;

namespace AlfaCharge.OcppServer.Hubs
{

    public class OcppEventsHub : Hub
    {
        // Optional grouping: clients can join per chargePointId to limit traffic
        public Task JoinChargePointGroup(string chargePointId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, $"cp:{chargePointId}");

        public Task LeaveChargePointGroup(string chargePointId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, $"cp:{chargePointId}");
    }
}