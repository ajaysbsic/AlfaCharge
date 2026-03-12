using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlfaCharge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BootNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargePointVendor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChargePointModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BootNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargingTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConnectorDbId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ocpp16TransactionId = table.Column<int>(type: "int", nullable: true),
                    Ocpp201TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StoppedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MeterStart = table.Column<long>(type: "bigint", nullable: true),
                    MeterStop = table.Column<long>(type: "bigint", nullable: true),
                    KWh = table.Column<double>(type: "float", nullable: true),
                    StopReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConnectorSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Available = table.Column<int>(type: "int", nullable: false),
                    Charging = table.Column<int>(type: "int", nullable: false),
                    Unavailable = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectorSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BusinessOwner = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NumberOfEvses = table.Column<int>(type: "int", nullable: false),
                    NumberOfConnectors_Id = table.Column<int>(type: "int", nullable: false),
                    NumberOfConnectors_Available = table.Column<int>(type: "int", nullable: false),
                    NumberOfConnectors_Charging = table.Column<int>(type: "int", nullable: false),
                    NumberOfConnectors_Unavailable = table.Column<int>(type: "int", nullable: false),
                    NumberOfConnectors_Total = table.Column<int>(type: "int", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.UniqueConstraint("AK_Locations_LocationId", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "OcppConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Readonly = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcppConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcppJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Retries = table.Column<int>(type: "int", nullable: true),
                    RetryInterval = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcppJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcppLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    MessageTypeId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcppLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcppVariableSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Component = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentInstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Variable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VariableInstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttributeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mutability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Persistent = table.Column<bool>(type: "bit", nullable: true),
                    Constant = table.Column<bool>(type: "bit", nullable: true),
                    SnapshotAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcppVariableSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Standard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UiName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UiNameTh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<int>(type: "int", nullable: false),
                    ManufacturerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxElectricPower = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationOverviewData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Offset = table.Column<int>(type: "int", nullable: false),
                    Limit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationOverviewData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ConnectorDbId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionMeterSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Measurand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionMeterSamples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Station_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LocationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargePoints_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChargePointId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPower = table.Column<double>(type: "float", nullable: false),
                    SecurityProtocol = table.Column<int>(type: "int", nullable: false),
                    QrCodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastOnline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StationOverviewDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Station_StationOverviewData_StationOverviewDataId",
                        column: x => x.StationOverviewDataId,
                        principalTable: "StationOverviewData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Connector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargePointDbId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectorNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OperationalStatus = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastStatusTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PowerType = table.Column<int>(type: "int", nullable: false),
                    PowerKw = table.Column<double>(type: "float", nullable: false),
                    StandardId = table.Column<int>(type: "int", nullable: false),
                    MaxVoltage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxAmperage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxElectricPower = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectorSequence = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connector_Standard_StandardId",
                        column: x => x.StandardId,
                        principalTable: "Standard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connector_Station_StationId",
                        column: x => x.StationId,
                        principalTable: "Station",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargePoints_ChargePointId",
                table: "ChargePoints",
                column: "ChargePointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChargePoints_LocationId",
                table: "ChargePoints",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargingTransactions_ChargePointId_StartedAt",
                table: "ChargingTransactions",
                columns: new[] { "ChargePointId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Connector_ChargePointDbId_ConnectorNumber",
                table: "Connector",
                columns: new[] { "ChargePointDbId", "ConnectorNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connector_StandardId",
                table: "Connector",
                column: "StandardId");

            migrationBuilder.CreateIndex(
                name: "IX_Connector_StationId",
                table: "Connector",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_LocationId",
                table: "Locations",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OcppLogs_ChargePointId_Timestamp",
                table: "OcppLogs",
                columns: new[] { "ChargePointId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Station_StationOverviewDataId",
                table: "Station",
                column: "StationOverviewDataId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistories_ChargePointId_OccurredAt",
                table: "StatusHistories",
                columns: new[] { "ChargePointId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionMeterSamples_TransactionId_Timestamp",
                table: "TransactionMeterSamples",
                columns: new[] { "TransactionId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BootNotifications");

            migrationBuilder.DropTable(
                name: "ChargePoints");

            migrationBuilder.DropTable(
                name: "ChargingTransactions");

            migrationBuilder.DropTable(
                name: "Connector");

            migrationBuilder.DropTable(
                name: "ConnectorSummaries");

            migrationBuilder.DropTable(
                name: "OcppConfigurations");

            migrationBuilder.DropTable(
                name: "OcppJobs");

            migrationBuilder.DropTable(
                name: "OcppLogs");

            migrationBuilder.DropTable(
                name: "OcppVariableSnapshots");

            migrationBuilder.DropTable(
                name: "StationModel");

            migrationBuilder.DropTable(
                name: "StatusHistories");

            migrationBuilder.DropTable(
                name: "TransactionMeterSamples");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Standard");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "StationOverviewData");
        }
    }
}
