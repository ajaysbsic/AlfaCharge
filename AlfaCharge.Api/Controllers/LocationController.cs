using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlfaCharge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly ILocationServices _locationServices;

        public LocationController(ILogger<LocationController> logger, ILocationServices locationServices)
        {
            _logger = logger;
            _locationServices = locationServices;
        }

        [HttpGet("Locations")]
        public async Task<IEnumerable<Location>> Get()
        {
            return await _locationServices.GetAllLocations(true);
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public async Task<Location> Get(string locationId)
        {
            return await _locationServices.GetLocationByID(locationId) ?? new Location();
        }

        // POST api/<LocationController>
        [HttpPost("AddLocation")]
        public async Task<Location> Post([FromBody] Location value)
        {
            return await _locationServices.AddLocation(value) ?? new Location();
        }

        // PUT api/<LocationController>/5
        [HttpPut("UpdateLocation")]
        public async Task<Location> Put(string id, [FromBody] string value)
        {
            return await _locationServices.UpdateLocation(id, new Location()) ?? new Location();
        }

        // DELETE api/<LocationController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(string locationId)
        {
            return await _locationServices.DeleteLocationByID(locationId);
        }
    }
}