using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.AbstractFactory;
using AlfaCharge.OcppServer.Versioned_Handlers;
using AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler;
using Microsoft.Extensions.DependencyInjection;

namespace AlfaCharge.OcppServer.Factory
{
    public sealed class Ocpp16HandlerFactory : IOcppHandlerFactory
    {
        private readonly IServiceProvider _provider;
        public Ocpp16HandlerFactory(IServiceProvider provider) => _provider = provider;

        public IBootNotificationHandler CreateBootNotificationHandler() =>
            _provider.GetRequiredService<Ocpp16BootNotificationHandler>();

        public IHeartbeatHandler CreateHeartbeatHandler() =>
            _provider.GetRequiredService<Ocpp16HeartbeatHandler>();

        public IAuthorizeHandler CreateAuthorizeHandler() =>
            _provider.GetRequiredService<Ocpp16AuthorizeHandler>();
        
        // StatusNotification (shared)
        public IStatusNotificationHandler CreateStatusNotificationHandler() =>
            _provider.GetRequiredService<IStatusNotificationHandler>();

        // Transactions (version-specific)
        public IOcpp16TransactionHandler Create16TransactionHandler() =>
            _provider.GetRequiredService<IOcpp16TransactionHandler>();

        public IOcpp201TransactionHandler Create201TransactionHandler() =>
            throw new NotImplementedException("2.x transactions not supported by 1.6 factory");

        // Configuration and Diagnostics
        public IConfigurationOps16 CreateConfigurationOps16()
            => _provider.GetRequiredService<Ocpp16ConfigurationHandler>();

        public IDiagnosticsFirmwareOps16 CreateDiagnosticsFirmwareOps16()
            => _provider.GetRequiredService<Ocpp16DiagnosticsFirmwareHandler>();

        public IConfigurationOps201 CreateConfigurationOps201()
            => throw new NotImplementedException("2.x configuration not supported by 1.6 factory");

        public IDiagnosticsFirmwareOps201 CreateDiagnosticsFirmwareOps201()
            => throw new NotImplementedException("2.x diagnostics not supported by 1.6 factory");

        // Remote Ops
        public IRemoteOps16 CreateRemoteOps16() => _provider.GetRequiredService<IRemoteOps16>();
        public IRemoteOps201 CreateRemoteOps201() => throw new NotImplementedException("2.x remote ops not supported by 1.6 factory");

        // Local Auth List Ops
        public ILocalAuthListOps16 CreateLocalAuthListOps16() => _provider.GetRequiredService<ILocalAuthListOps16>();
        public ILocalAuthListOps201 CreateLocalAuthListOps201() => throw new NotImplementedException("2.x local list ops not supported by 1.6 factory");

        // Reservation Ops
        public IReservationOps16 CreateReservationOps16() => _provider.GetRequiredService<IReservationOps16>();
        public IReservationOps201 CreateReservationOps201() => throw new NotImplementedException("2.x reservation ops not supported by 1.6 factory");

        // Charging Profile Ops
        public IChargingProfileOps16 CreateChargingProfileOps16() => _provider.GetRequiredService<IChargingProfileOps16>();
        public IChargingProfileOps201 CreateChargingProfileOps201() => throw new NotImplementedException("2.x charging profile ops not supported by 1.6 factory");

        // Triggers Ops
        public ITriggersOps16 CreateTriggersOps16() => _provider.GetRequiredService<ITriggersOps16>();
        public ITriggersOps201 CreateTriggersOps201() => throw new NotImplementedException("2.x triggers ops not supported by 1.6 factory");
    }
}