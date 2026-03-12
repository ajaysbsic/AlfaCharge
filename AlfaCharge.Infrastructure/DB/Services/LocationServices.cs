using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Infrastructure.DB.Services
{
    public class LocationServices : ILocationServices
    {
        private readonly ApplicationDbContext _locationDbContext;
        public LocationServices(ApplicationDbContext db)
        {
            this._locationDbContext = db;
        }

        public async Task<Location?> AddLocation(Location locationObject)
        {
            var result = await _locationDbContext.Locations.AddAsync(locationObject);
            await _locationDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> DeleteLocationByID(string locationId)
        {
            var result = await _locationDbContext.Locations.FirstOrDefaultAsync(x=> x.LocationId == locationId);

            if(result != null)
            {
                _locationDbContext.Locations.Remove(result);
                await _locationDbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Location>> GetAllLocations(bool? isActive)
        {
            // Note: isActive parameter is currently not used. Implement filtering logic if needed.
            return await _locationDbContext.Locations.ToListAsync();
        }

        public async Task<Location?> GetLocationByID(string locationId)
        {
            // Include ChargePoints to load related data
            return await _locationDbContext.Locations
                .Include(x => x.ChargePoints)
                .FirstOrDefaultAsync(x => x.LocationId == locationId);
        }

        public async Task<Location?> UpdateLocation(string id, Location locationObject)
        {
            var existingLocation = await _locationDbContext.Locations
                .Include(l => l.ChargePoints) // Include related entities if you want to update them as well
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (existingLocation == null)
            {
                return null; // Or throw exception if preferred
            }

            // Update scalar/owned properties
            existingLocation.LocationName = locationObject.LocationName;
            existingLocation.BusinessName = locationObject.BusinessName;
            existingLocation.NumberOfEvses = locationObject.NumberOfEvses;

            // Update owned LocationCoordinates
            existingLocation.Latitude = locationObject?.Latitude;
            existingLocation.Longitude = locationObject?.Longitude;

            // Update owned NumberOfConnectors
            if (existingLocation.NumberOfConnectors == null)
            {
                existingLocation.NumberOfConnectors = new Domain.Models.NumberOfConnectors();
            }
            existingLocation.NumberOfConnectors.Available = locationObject.NumberOfConnectors?.Available ?? 0;
            existingLocation.NumberOfConnectors.Charging = locationObject.NumberOfConnectors?.Charging ?? 0;
            existingLocation.NumberOfConnectors.Unavailable = locationObject.NumberOfConnectors?.Unavailable ?? 0;
            existingLocation.NumberOfConnectors.Total = locationObject.NumberOfConnectors?.Total ?? 0;

            // Optional: Update ChargePoints collection if needed
            // This can be complex depending on requirements (add/remove/update), here’s a simple replace example:
            existingLocation.ChargePoints.Clear();
            if (locationObject.ChargePoints != null)
            {
                foreach (var cp in locationObject.ChargePoints)
                {
                    existingLocation.ChargePoints.Add(cp);
                }
            }

            // Mark the entity as modified (EF Core tracks changes automatically, but explicit is safer)
            _locationDbContext.Locations.Update(existingLocation);

            await _locationDbContext.SaveChangesAsync();

            return existingLocation;
        }
    }
}
