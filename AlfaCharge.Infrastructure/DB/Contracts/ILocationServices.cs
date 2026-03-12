
using AlfaCharge.Domain.Entities;

namespace AlfaCharge.Infrastructure.DB.Contracts
{
    public interface ILocationServices
    {
        Task<List<Location>> GetAllLocations(bool? isActive);

        Task<Location?> GetLocationByID(string locationId);

        Task<Location?> AddLocation(Location locationObject);

        Task<Location?> UpdateLocation(string id, Location locationObject);
        
        Task<bool> DeleteLocationByID(string locationId);

    }
}
