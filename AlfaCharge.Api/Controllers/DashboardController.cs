using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlfaCharge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DashboardController : ControllerBase
    {
        // Deprecated placeholder controller kept for backward compatibility.
        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint. Use /api/metrics/dashboard instead."
            });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint. Use /api/metrics/* endpoints instead."
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint."
            });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint."
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint."
            });
        }
    }
}
