using AlfaCharge.Domain.Entities;
using AlfaCharge.Domain.Models;
using AlfaCharge.Domain.Models.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AlfaCharge.Infrastructure.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Station { get; set; }
        public DbSet<Connector> Connector { get; set; }
        public DbSet<ConnectorSummary> ConnectorSummaries { get; set; }
        public DbSet<StationModel> StationModel { get; set; }
        public DbSet<Standard> Standard { get; set; }
        public DbSet<Station> StationOverviewItem { get; set; }
        public DbSet<StationOverviewData> StationOverviewData { get; set; }

        // For WebSockets
        public DbSet<ChargePointBootNotification> BootNotifications { get; set; }
        
        //Entities
        public DbSet<Connector> Connectors => Set<Connector>();
        public DbSet<OCPPLog> OcppLogs => Set<OCPPLog>();
        public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
        public DbSet<ChargePoint> ChargePoints => Set<ChargePoint>();
        public DbSet<Domain.Entities.Location> Locations => Set<Domain.Entities.Location>();
        public DbSet<ChargingTransaction> ChargingTransactions => Set<ChargingTransaction>();
        public DbSet<TransactionMeterSample> TransactionMeterSamples => Set<TransactionMeterSample>();

        public DbSet<OcppConfigurationEntry> OcppConfigurations => Set<OcppConfigurationEntry>();
        public DbSet<OcppVariableSnapshot201> OcppVariableSnapshots => Set<OcppVariableSnapshot201>();
        public DbSet<OcppJob> OcppJobs => Set<OcppJob>();

        //For OCPP logs
        public DbSet<OcppFrameLog> OcppFrameLogs => Set<OcppFrameLog>();

        // Admin entities
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<RfidCard> RfidCards => Set<RfidCard>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OCPPLog>()
                            .HasIndex(x => new { x.ChargePointId, x.Timestamp });

            modelBuilder.Entity<Connector>()
                .HasIndex(x => new { x.ChargePointDbId, x.ConnectorNumber })
                .IsUnique();

            modelBuilder.Entity<StatusHistory>()
                .HasIndex(x => new { x.ChargePointId, x.OccurredAt });

            modelBuilder.Entity<ChargePoint>()
                .HasIndex(x => x.ChargePointId)
                .IsUnique();

            var statusConverter = new EnumToStringConverter<ConnectorStatus>();
            modelBuilder.Entity<Connector>()
                .Property(x => x.Status)
                .HasConversion(statusConverter)
                .HasMaxLength(64);


            // ChargePointId and LocationId unique
            modelBuilder.Entity<ChargePoint>()
                .HasIndex(x => x.ChargePointId)
                .IsUnique();

            modelBuilder.Entity<Domain.Entities.Location>()
                .HasIndex(x => x.LocationId)
                .IsUnique();

            // Map ChargePoint.LocationId (string) -> Location.LocationId (string) as principal key
            modelBuilder.Entity<ChargePoint>()
                .HasOne<Domain.Entities.Location>()
                .WithMany(l => l.ChargePoints)
                .HasForeignKey(cp => cp.LocationId)
                .HasPrincipalKey(l => l.LocationId);

            modelBuilder.Entity<ChargingTransaction>()
                .HasIndex(x => new { x.ChargePointId, x.StartedAt });

            modelBuilder.Entity<TransactionMeterSample>()
                .HasIndex(x => new { x.TransactionId, x.Timestamp });

            //TODO
            modelBuilder.Entity<Location>()
                .OwnsOne(l => l.NumberOfConnectors, noc =>
                {
                    noc.Property(p => p.Available).HasColumnName("NumberOfConnectors_Available");
                    noc.Property(p => p.Charging).HasColumnName("NumberOfConnectors_Charging");
                    noc.Property(p => p.Unavailable).HasColumnName("NumberOfConnectors_Unavailable");
                    noc.Property(p => p.Total).HasColumnName("NumberOfConnectors_Total");
                });

            // AppUser configuration
            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.Email)
                .IsUnique();

            // RfidCard configuration
            modelBuilder.Entity<RfidCard>()
                .HasIndex(x => x.IdTag)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}