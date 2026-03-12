using AlfaCharge.OcppServer.Contracts.DTO.DiagnosticsFirmware;
using AlfaCharge.OcppServer.Contracts.DTO.Message201;
using AlfaCharge.OcppServer.Contracts.DTO.Messages;
using AlfaCharge.OcppServer.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AlfaCharge.Api.Controllers
{
    [ApiController]
    [Route("api/ocpp/{chargePointId}/ops")]
    public class DiagnosticsFirmwareController : ControllerBase
    {
        private readonly IConfigurationOps16 _ops16;
        private readonly IConfigurationOps201 _ops201;
        private readonly IDiagnosticsFirmwareOps16 _diag16;
        private readonly IDiagnosticsFirmwareOps201 _diag201;

        public DiagnosticsFirmwareController(
            IConfigurationOps16 ops16,
            IConfigurationOps201 ops201,
            IDiagnosticsFirmwareOps16 diag16,
            IDiagnosticsFirmwareOps201 diag201)
        {
            _ops16 = ops16;
            _ops201 = ops201;
            _diag16 = diag16;
            _diag201 = diag201;
        }

        [HttpPost("16/get-configuration")]
        public Task<GetConfigurationConf16> GetConfiguration16(string chargePointId, [FromBody] string[]? keys, CancellationToken ct)
            => _ops16.GetConfigurationAsync(chargePointId, keys, ct);

        [HttpPost("16/change-configuration")]
        public Task<ChangeConfigurationConf16> ChangeConfiguration16(string chargePointId, [FromBody] ChangeConfigurationReq16 req, CancellationToken ct)
            => _ops16.ChangeConfigurationAsync(chargePointId, req.Key, req.Value, ct);

        [HttpPost("201/get-variables")]
        public Task<GetVariablesConf201> GetVariables201(string chargePointId, [FromBody] GetVariablesReq201 req, CancellationToken ct)
            => _ops201.GetVariablesAsync(chargePointId, req, ct);

        [HttpPost("201/set-variables")]
        public Task<SetVariablesConf201> SetVariables201(string chargePointId, [FromBody] SetVariablesReq201 req, CancellationToken ct)
            => _ops201.SetVariablesAsync(chargePointId, req, ct);

        [HttpPost("201/get-base-report")]
        public Task<GetBaseReportConf201> GetBaseReport201(string chargePointId, [FromBody] GetBaseReportReq201 req, CancellationToken ct)
            => _ops201.GetBaseReportAsync(chargePointId, req, ct);

        [HttpPost("16/get-diagnostics")]
        public Task<GetDiagnosticsConf16> GetDiagnostics16(string chargePointId, [FromBody] GetDiagnosticsReq16 req, CancellationToken ct)
            => _diag16.GetDiagnosticsAsync(chargePointId, req, ct);

        [HttpPost("16/update-firmware")]
        public Task<UpdateFirmwareConf16> UpdateFirmware16(string chargePointId, [FromBody] UpdateFirmwareReq16 req, CancellationToken ct)
            => _diag16.UpdateFirmwareAsync(chargePointId, req, ct);

        [HttpPost("201/get-log")]
        public Task<GetLogConf201> GetLog201(string chargePointId, [FromBody] GetLogReq201 req, CancellationToken ct)
            => _diag201.GetLogAsync(chargePointId, req, ct);

        [HttpPost("201/update-firmware")]
        public Task<UpdateFirmwareConf201> UpdateFirmware201(string chargePointId, [FromBody] UpdateFirmwareReq201 req, CancellationToken ct)
            => _diag201.UpdateFirmwareAsync(chargePointId, req, ct);
    }
}