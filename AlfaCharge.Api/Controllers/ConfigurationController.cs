using AlfaCharge.OcppServer.Contracts.DTO.Message201;
using AlfaCharge.OcppServer.Contracts.DTO.Messages;
using AlfaCharge.OcppServer.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AlfaCharge.Api.Controllers
{
    [ApiController]
    [Route("api/ocpp/{chargePointId}/config")]
    //[Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationOps16 _ops16;
        private readonly IConfigurationOps201 _ops201;

        public ConfigurationController(IConfigurationOps16 ops16, IConfigurationOps201 ops201)
        {
            _ops16 = ops16;
            _ops201 = ops201;
        }

        [HttpPost("1.6/get")]
        public async Task<ActionResult<GetConfigurationConf16>> GetConfiguration16(
            string chargePointId, [FromBody] string[]? keys, CancellationToken ct)
        {
            var conf = await _ops16.GetConfigurationAsync(chargePointId, keys, ct);
            return Ok(conf);
        }

        [HttpPost("1.6/change")]
        public async Task<ActionResult<ChangeConfigurationConf16>> ChangeConfiguration16(
            string chargePointId, [FromBody] ChangeConfigurationReq16 req, CancellationToken ct)
        {
            var conf = await _ops16.ChangeConfigurationAsync(chargePointId, req.Key, req.Value, ct);
            return Ok(conf);
        }

        [HttpPost("2.0/get-variables")]
        public async Task<ActionResult<GetVariablesConf201>> GetVariables20(
            string chargePointId, [FromBody] GetVariablesReq201 req, CancellationToken ct)
        {
            var conf = await _ops201.GetVariablesAsync(chargePointId, req, ct);
            return Ok(conf);
        }

        [HttpPost("2.0/set-variables")]
        public async Task<ActionResult<SetVariablesConf201>> SetVariables20(
            string chargePointId, [FromBody] SetVariablesReq201 req, CancellationToken ct)
        {
            var conf = await _ops201.SetVariablesAsync(chargePointId, req, ct);
            return Ok(conf);
        }

        [HttpPost("2.0/get-base-report")]
        public async Task<ActionResult<GetBaseReportConf201>> GetBaseReport20(
            string chargePointId, [FromBody] GetBaseReportReq201 req, CancellationToken ct)
        {
            var conf = await _ops201.GetBaseReportAsync(chargePointId, req, ct);
            return Ok(conf);
        }
    }
}