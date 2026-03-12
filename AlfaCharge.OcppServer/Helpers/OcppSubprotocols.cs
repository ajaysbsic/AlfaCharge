namespace AlfaCharge.OcppServer.Helpers
{

    public static class OcppSubprotocols
    {
        public const string Ocpp16 = "ocpp1.6";
        public const string Ocpp201 = "ocpp2.0.1";

        public static bool TryParse(string subProtocol, out OcppProtocolVersion version)
        {
            switch (subProtocol)
            {
                case Ocpp16:
                    version = OcppProtocolVersion.Ocpp16;
                    return true;
                case Ocpp201:
                    version = OcppProtocolVersion.Ocpp201;
                    return true;
                default:
                    version = default;
                    return false;
            }
        }
    }
}