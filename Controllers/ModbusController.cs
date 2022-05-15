using Microsoft.AspNetCore.Mvc;
using ModbusGateWay.Modbus;

namespace ModbusGateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModbusController : ControllerBase
    {
        private readonly ILogger<ModbusController> _logger;
        private readonly ActualData _actualData;

        public ModbusController(ILogger<ModbusController> logger, ActualData actualData)
        {
            _logger = logger;

            _actualData = actualData;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            yield return _actualData.CurrentChanel1.ToString("0.00");
            yield return _actualData.CurrentChanel2.ToString("0.00");
            yield return _actualData.CurrentChanel3.ToString("0.00");
            yield return _actualData.CurrentChanel4.ToString("0.00");
        }
    }
}