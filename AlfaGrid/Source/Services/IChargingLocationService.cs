using System.Collections.Generic;
using System.Threading.Tasks;
using AlfaGrid.Source.Models;

namespace AlfaGrid.Source.Services
{
    public interface IChargingLocationService
    {
        Task<List<ChargingLocation>> GetLocationsAsync();
        Task<List<ChargingStation>> GetStationsAsync();
        Task<ChargingLocation> GetLocationByIdAsync(string locationId);
        Task<List<ChargingLocation>> GetLocationsWithStationsAsync();
    }
}
