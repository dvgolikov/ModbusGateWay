using Microsoft.AspNetCore.Mvc;
using ModbusGateWay.Modbus;

namespace ModbusGateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModbusController : ControllerBase
    {
        private readonly ILogger<ModbusController> _logger;

        public ModbusController(ILogger<ModbusController> logger)
        {
            _logger = logger;

        }

        [HttpGet]
        public IActionResult Get([FromBody] RequestParameters requestParameters, DataCaptureService dataCaptureService)
        {
            
        }
    }
}