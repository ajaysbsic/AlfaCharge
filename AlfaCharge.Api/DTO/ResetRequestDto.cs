namespace AlfaCharge.Api.DTO
{
    public sealed class ResetRequestDto
    {
        /// <summary>
        /// OCPP 1.6: "Hard" | "Soft"
        /// OCPP 2.0.1: "Immediate" | "OnIdle"
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// OCPP 2.0.1 only. If provided, applies reset to a specific EVSE.
        /// </summary>
        public int? EvseId { get; set; }

        /// <summary>
        /// Optional timeout (seconds) to wait for charger CALLRESULT/CALLERROR. Default 30s.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
