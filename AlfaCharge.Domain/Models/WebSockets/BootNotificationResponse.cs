namespace AlfaCharge.Domain.Models.WebSockets
{
    public class BootNotificationResponse
    {
        public NotificationStatus Status { get; set; }
        public DateTime CurrentTime { get; set; }
        public int Interval { get; set; }
        public StatusInfo StatusInfo { get; set; }
    }
}