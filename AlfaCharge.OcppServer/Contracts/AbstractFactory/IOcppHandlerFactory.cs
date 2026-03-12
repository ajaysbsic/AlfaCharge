namespace AlfaCharge.OcppServer.Contracts.AbstractFactory
{
    public interface IOcppHandlerFactory
    {
        IBootNotificationHandler CreateBootNotificationHandler();
        IHeartbeatHandler CreateHeartbeatHandler();
        IAuthorizeHandler CreateAuthorizeHandler();

        // StatusNotification (shared handler supports both 1.6 & 2.0.1/2.1)
        IStatusNotificationHandler CreateStatusNotificationHandler();

        // Transactions (version-specific)
        IOcpp16TransactionHandler Create16TransactionHandler();
        IOcpp201TransactionHandler Create201TransactionHandler();

        // Configuration and Diagnostics
        IConfigurationOps16 CreateConfigurationOps16();
        IDiagnosticsFirmwareOps16 CreateDiagnosticsFirmwareOps16();
        IConfigurationOps201 CreateConfigurationOps201();
        IDiagnosticsFirmwareOps201 CreateDiagnosticsFirmwareOps201();

        // Remote operations (CSMS -> CP)
        IRemoteOps16 CreateRemoteOps16();
        IRemoteOps201 CreateRemoteOps201();

        // Local auth list operations (CSMS -> CP)
        ILocalAuthListOps16 CreateLocalAuthListOps16();
        ILocalAuthListOps201 CreateLocalAuthListOps201();

        // Reservation operations (CSMS -> CP)
        IReservationOps16 CreateReservationOps16();
        IReservationOps201 CreateReservationOps201();

        // Charging profile operations (CSMS -> CP)
        IChargingProfileOps16 CreateChargingProfileOps16();
        IChargingProfileOps201 CreateChargingProfileOps201();

        // Triggers (CSMS -> CP)
        ITriggersOps16 CreateTriggersOps16();
        ITriggersOps201 CreateTriggersOps201();
    }
}