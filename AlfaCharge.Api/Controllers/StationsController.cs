using AlfaCharge.Domain.Models;
using AlfaCharge.Infrastructure.DB.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AlfaCharge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly ILogger<StationsController> _logger;
        private readonly IStationServices _stationServices;

        public StationsController(ILogger<StationsController> logger, IStationServices stationServices)
        {
            _logger = logger;
            _stationServices = stationServices;
        }

        [HttpGet("Stations")]
        public async Task<IEnumerable<Station>> Get()
        {
            return await _stationServices.GetAllStations(true);
        }

        [HttpGet("StationById")]
        public async Task<Station> Get(int stationId)
        {
            return await _stationServices.GetStationByID(stationId) ?? new Station();
        }

        [HttpPost("AddStation")]
        public async Task<Station> Post([FromBody] Station value)
        {
            return await _stationServices.AddStation(value) ?? new Station();
        }

        [HttpPut("UpdateStation")]
        public async Task<Station> Put(int id, [FromBody] string value)
        {
            return await _stationServices.UpdateStation(id, new Station()) ?? new Station();
        }

        [HttpPost("DeleteStation")]
        public async Task<bool> Delete(int stationId)
        {
            return await _stationServices.DeleteStationByID(stationId);
        }
    }
}
