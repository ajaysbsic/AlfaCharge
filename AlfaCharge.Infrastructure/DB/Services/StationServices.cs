using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Infrastructure.DB.Services
{
    public class StationServices : IStationServices
    {
        private readonly ApplicationDbContext _stationDbContext;
        public StationServices(ApplicationDbContext db)
        {
            _stationDbContext = db;
        }

        public async Task<Station?> AddStation(Station stationObject)
        {
            var entity = await _stationDbContext.Station.AddAsync(stationObject);
            await _stationDbContext.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task<bool> DeleteStationByID(int stationId)
        {
            var station = await _stationDbContext.Station.FirstOrDefaultAsync(s => s.Id == stationId);
            if (station is null)
                return false;

            _stationDbContext.Station.Remove(station);
            await _stationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Station>> GetAllStations(bool? isActive)
        {
            // Current model does not expose IsActive; return all records.
            return await _stationDbContext.Station.ToListAsync();
        }

        public async Task<List<Standard>> GetAllStationTypes(bool? isActive)
        {
            var query = _stationDbContext.Standard.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.ToListAsync();
        }

        public async Task<List<ConnectorSummary>> GetConnectorSummaryByStationId(int stationId)
        {
            var station = await _stationDbContext.Station
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == stationId);

            if (station is null)
                return new List<ConnectorSummary>();

            var cp = await _stationDbContext.ChargePoints
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChargePointId == station.ChargePointId);

            if (cp is null)
                return new List<ConnectorSummary>();

            var summary = await _stationDbContext.Connectors
                .Where(c => c.ChargePointDbId == cp.Id)
                .GroupBy(_ => 1)
                .Select(g => new ConnectorSummary
                {
                    Id = stationId,
                    Available = g.Count(c => c.Status == ConnectorStatus.Available),
                    Charging = g.Count(c => c.Status == ConnectorStatus.Charging),
                    Unavailable = g.Count(c => c.Status == ConnectorStatus.Unavailable),
                    Total = g.Count()
                })
                .FirstOrDefaultAsync();

            return summary is null ? new List<ConnectorSummary>() : new List<ConnectorSummary> { summary };
        }

        public async Task<Station?> GetStationByID(int stationId)
        {
            return await _stationDbContext.Station.FirstOrDefaultAsync(s => s.Id == stationId);
        }

        public async Task<List<Station>> GetStationsByLocationId(int locationId)
        {
            return await _stationDbContext.Station
                .Where(s => s.LocationId == locationId.ToString())
                .ToListAsync();
        }

        public async Task<List<Station>> GetStationsByModelId(int modelId)
        {
            var stationModel = await _stationDbContext.StationModel
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == modelId);

            if (stationModel is null)
                return new List<Station>();

            return await _stationDbContext.Station
                .Where(s => s.Model == stationModel.Name)
                .ToListAsync();
        }

        public async Task<Station?> UpdateStation(int id, Station stationObject)
        {
            var existing = await _stationDbContext.Station.FirstOrDefaultAsync(s => s.Id == id);
            if (existing is null)
                return null;

            existing.StationName = stationObject.StationName;
            existing.ChargePointId = stationObject.ChargePointId;
            existing.LocationId = stationObject.LocationId;
            existing.Status = stationObject.Status;
            existing.Model = stationObject.Model;
            existing.MaxPower = stationObject.MaxPower;
            existing.SecurityProtocol = stationObject.SecurityProtocol;
            existing.QrCodeUrl = stationObject.QrCodeUrl;
            existing.FirmwareVersion = stationObject.FirmwareVersion;
            existing.SerialNumber = stationObject.SerialNumber;
            existing.LastOnline = stationObject.LastOnline;
            existing.Connectors = stationObject.Connectors;

            await _stationDbContext.SaveChangesAsync();
            return existing;
        }
    }
}
