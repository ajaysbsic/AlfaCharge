using AlfaCharge.Domain.Models;

namespace AlfaCharge.Infrastructure.DB.Contracts
{
    public interface IStationServices
    {
        Task<List<ConnectorSummary>> GetConnectorSummaryByStationId(int stationId);

        Task<List<Standard>> GetAllStationTypes(bool? isActive);

        Task<List<Station>> GetAllStations(bool? isActive);

        Task<Station?> GetStationByID(int stationId);

        Task<Station?> AddStation(Station stationObject);

        Task<Station?> UpdateStation(int id, Station stationObject);

        Task<bool> DeleteStationByID(int stationId);

        Task<List<Station>> GetStationsByLocationId(int locationId);

        Task<List<Station>> GetStationsByModelId(int modelId);

        //Task<List<StationOverviewItem>> GetStationsByBusinessId(int businessId);
        //Task<List<StationOverviewItem>> GetStationsByCity(string city);
        //Task<List<StationOverviewItem>> GetStationsByState(string state);

    }
}
