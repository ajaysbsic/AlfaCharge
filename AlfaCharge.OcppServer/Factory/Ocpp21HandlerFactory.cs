using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Contracts.AbstractFactory;
using AlfaCharge.OcppServer.Versioned_Handlers;
using AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler;
using Microsoft.Extensions.DependencyInjection;

namespace AlfaCharge.OcppServer.Factory
{
    public sealed class Ocpp21HandlerFactory : IOcppHandlerFactory
    {
        private readonly IServiceProvider _provider;
        public Ocpp21HandlerFactory(IServiceProvider provider) => _provider = provider;

        public IBootNotificationHandler CreateBootNotificationHandler() =>
            _provider.GetRequiredService<Ocpp21BootNotificationHandler>();

        public IHeartbeatHandler CreateHeartbeatHandler() =>
            _provider.GetRequiredService<Ocpp21HeartbeatHandler>();

        public IAuthorizeHandler CreateAuthorizeHandler() =>
            _provider.GetRequiredService<Ocpp21AuthorizeHandler>();

        // shared handler for both versions
        public IStatusNotificationHandler CreateStatusNotificationHandler() =>
            _provider.GetRequiredService<IStatusNotificationHandler>();

        // version-specific
        public IOcpp16TransactionHandler Create16TransactionHandler() =>
            throw new NotImplementedException("1.6 transactions not supported by 2.x factory");

        public IOcpp201TransactionHandler Create201TransactionHandler() =>
            _provider.GetRequiredService<IOcpp201TransactionHandler>();

        // Configuration and Diagnostics
        public IConfigurationOps16 CreateConfigurationOps16()
            => throw new NotImplementedException("1.6 configuration not supported by 2.x factory");

        public IDiagnosticsFirmwareOps16 CreateDiagnosticsFirmwareOps16()
            => throw new NotImplementedException("1.6 diagnostics not supported by 2.x factory");

        public IConfigurationOps201 CreateConfigurationOps201()
            => _provider.GetRequiredService<Ocpp201ConfigurationHandler>();

        public IDiagnosticsFirmwareOps201 CreateDiagnosticsFirmwareOps201()
            => _provider.GetRequiredService<Ocpp201DiagnosticsFirmwareHandler>();

        // Remote Ops
        public IRemoteOps16 CreateRemoteOps16() => throw new NotImplementedException("1.6 remote ops not supported by 2.x factory");
        public IRemoteOps201 CreateRemoteOps201() => _provider.GetRequiredService<IRemoteOps201>();

        // Local Auth List Ops
        public ILocalAuthListOps16 CreateLocalAuthListOps16() => throw new NotImplementedException("1.6 local list ops not supported by 2.x factory");
        public ILocalAuthListOps201 CreateLocalAuthListOps201() => _provider.GetRequiredService<ILocalAuthListOps201>();

        // Reservation Ops
        public IReservationOps16 CreateReservationOps16() => throw new NotImplementedException("1.6 reservation ops not supported by 2.x factory");
        public IReservationOps201 CreateReservationOps201() => _provider.GetRequiredService<IReservationOps201>();

        // Charging Profile Ops
        public IChargingProfileOps16 CreateChargingProfileOps16() => throw new NotImplementedException("1.6 charging profile ops not supported by 2.x factory");
        public IChargingProfileOps201 CreateChargingProfileOps201() => _provider.GetRequiredService<IChargingProfileOps201>();

        // Triggers Ops
        public ITriggersOps16 CreateTriggersOps16() => throw new NotImplementedException("1.6 triggers ops not supported by 2.x factory");
        public ITriggersOps201 CreateTriggersOps201() => _provider.GetRequiredService<ITriggersOps201>();
    }
}