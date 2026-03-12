using AlfaCharge.Domain.Models.WebSockets;
using AlfaCharge.Infrastructure.DB.Contracts;

namespace AlfaCharge.Infrastructure.DB.Services
{
    public class BootNotificationService : IBootNotificationService
    {
        private readonly ApplicationDbContext _context;

        public BootNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChargePointBootNotification> SaveAsync(BootNotificationRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var model = request.ChargingStation?.Model ?? "unknown-model";

                var vendor = request.ChargingStation?.VendorName ?? "unknown-vendor";
                var serial = request.ChargingStation?.SerialNumber ?? "unknown-serial";
                var fw = request.ChargingStation?.FirmwareVersion ?? "unknown-firmware";
                var reason = request.Reason ?? "test_reason";
                if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(vendor))
                {
                    // Defaulting is safer during bring-up:
                    model ??= "unknown-model";
                    vendor ??= "unknown-vendor";
                }

                var entity = new ChargePointBootNotification
                {
                    ChargePointModel = model,
                    ChargePointVendor = vendor,
                    SerialNumber = serial,
                    FirmwareVersion = fw,
                    Reason = reason,
                    ReceivedAt = DateTime.UtcNow
                };

                _context.BootNotifications.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}