using AlfaCharge.OcppServer.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AlfaCharge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OCPPController : ControllerBase
    {
        private readonly IAuthorizeHandler _authorizeHandler;
        public OCPPController(IAuthorizeHandler authorizeHandler)
        {
            this._authorizeHandler = authorizeHandler;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint. Use /api/cp and /api/admin/stations/*/actions instead."
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await _authorizeHandler.HandleAsync(new Domain.Models.WebSockets.AuthorizeRequest { IdTag = "12345" });
            return StatusCode(StatusCodes.Status410Gone, new
            {
                message = "Deprecated endpoint. Authorization test action moved to protocol handlers."
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
