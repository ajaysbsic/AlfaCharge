using System.Text.Json;
using System.Text.Json.Serialization;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.Infrastructure.DB.Contracts;
using AlfaCharge.Infrastructure.DB.Services;
using AlfaCharge.OcppServer.Versioned_Handlers;
using AlfaCharge.OcppServer.Versioned_Handlers.ConfigurationHandler;
using AlfaCharge.OcppServer.WebSockets;
using Microsoft.EntityFrameworkCore;
using AlfaCharge.OcppServer.Versioned_Handlers.Ocpp16;
using AlfaCharge.OcppServer.Versioned_Handlers.Ocpp201;
using AlfaCharge.OcppServer.Contracts;
using AlfaCharge.OcppServer.Services;
using AlfaCharge.OcppServer.Versioned_Handlers.TransactionHandlers;
using AlfaCharge.OcppServer.Hubs;
using AlfaCharge.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AlfaCharge API",
        Version = "v1",
        Description = "API documentation for AlfaCharge WebAPI"
    });
});

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Domain services
builder.Services.AddScoped<ILocationServices, LocationServices>();
builder.Services.AddScoped<IOCPPServices, OCPPServices>();
builder.Services.AddScoped<IStationServices, StationServices>();
builder.Services.AddScoped<IBootNotificationService, BootNotificationService>();
builder.Services.AddScoped<IMetricsQueryService, MetricsQueryService>();

// (OCPP) WebSocket server & infrastructure
builder.Services.AddSingleton<OcppConnectionManager>();
builder.Services.AddSingleton<OcppWebSocketServer>();

// JSON options for OCPP frames: camelCase and string enums
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
});

// Version-specific handlers
builder.Services.AddScoped<Ocpp16BootNotificationHandler>();
builder.Services.AddScoped<Ocpp16HeartbeatHandler>();
builder.Services.AddScoped<Ocpp16AuthorizeHandler>();
builder.Services.AddScoped<Ocpp21BootNotificationHandler>();
builder.Services.AddScoped<Ocpp21HeartbeatHandler>();
builder.Services.AddScoped<Ocpp21AuthorizeHandler>();
// Configuration & Diagnostics handlers
builder.Services.AddScoped<Ocpp16ConfigurationHandler>();
builder.Services.AddScoped<Ocpp16DiagnosticsFirmwareHandler>();
builder.Services.AddScoped<Ocpp201ConfigurationHandler>();
builder.Services.AddScoped<Ocpp201DiagnosticsFirmwareHandler>();
builder.Services.AddScoped<IConfigurationOps16, Ocpp16ConfigurationHandler>();
builder.Services.AddScoped<IConfigurationOps201, Ocpp201ConfigurationHandler>();
builder.Services.AddScoped<IDiagnosticsFirmwareOps16, Ocpp16DiagnosticsFirmwareHandler>();
builder.Services.AddScoped<IDiagnosticsFirmwareOps201, Ocpp201DiagnosticsFirmwareHandler>();

// Message routing handlers resolved by interface in factories
builder.Services.AddScoped<IStatusNotificationHandler, StatusNotificationHandler>();
builder.Services.AddScoped<IOcpp16TransactionHandler, Ocpp16TransactionHandler>();
builder.Services.AddScoped<IOcpp201TransactionHandler, Ocpp201TransactionHandler>();

// New ops registrations
builder.Services.AddScoped<IRemoteOps16, Ocpp16RemoteOpsHandler>();
builder.Services.AddScoped<IRemoteOps201, Ocpp201RemoteOpsHandler>();
builder.Services.AddScoped<ILocalAuthListOps16, Ocpp16LocalListHandler>();
builder.Services.AddScoped<ILocalAuthListOps201, Ocpp201LocalListHandler>();
builder.Services.AddScoped<IReservationOps16, Ocpp16ReservationHandler>();
builder.Services.AddScoped<IReservationOps201, Ocpp201ReservationHandler>();
builder.Services.AddScoped<IChargingProfileOps16, Ocpp16ChargingProfileHandler>();
builder.Services.AddScoped<IChargingProfileOps201, Ocpp201ChargingProfileHandler>();
builder.Services.AddScoped<ITriggersOps16, Ocpp16TriggersHandler>();
builder.Services.AddScoped<ITriggersOps201, Ocpp201TriggersHandler>();

// Add SignalR
builder.Services.AddSignalR();

// OCPP Log Writer (batched background persistence)
builder.Services.AddSingleton<BatchedOcppLogWriter>();
builder.Services.AddSingleton<IOcppLogWriter>(sp => sp.GetRequiredService<BatchedOcppLogWriter>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<BatchedOcppLogWriter>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseWebSockets();

app.UseAuthorization();

app.MapControllers();
app.MapHub<OcppEventsHub>("/hub/ocpp");

// Map OCPP endpoint
var ocppServer = app.Services.GetRequiredService<OcppWebSocketServer>();
ocppServer.MapOcppEndpoint(app);

app.Run();