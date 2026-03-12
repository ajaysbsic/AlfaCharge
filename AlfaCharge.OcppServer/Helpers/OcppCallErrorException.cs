namespace AlfaCharge.OcppServer.Helpers
{

    public sealed class OcppCallErrorException : Exception
    {
        public string ErrorCode { get; }
        public string MessageId { get; }
        public string? DetailsJson { get; }

        public OcppCallErrorException(string messageId, string errorCode, string description, string? detailsJson = null)
            : base(description)
        {
            MessageId = messageId;
            ErrorCode = errorCode;
            DetailsJson = detailsJson;
        }
    }
}