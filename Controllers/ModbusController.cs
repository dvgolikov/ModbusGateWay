using Microsoft.AspNetCore.Mvc;
using ModbusGateWay.Models;

namespace ModbusGateWay.Controllers
{
    public class ModbusController : ControllerBase
    {
        private readonly ILogger<ModbusController> _logger;
        private readonly DataCollector dataCollector;

        public ModbusController(ILogger<ModbusController> logger, DataCollector dataCollector)
        {
            _logger = logger;
            this.dataCollector = dataCollector;
        }

        public IEnumerable<string> GetPorts()
        {
            return dataCollector.GetAvailableComPort();
        }

        public IEnumerable<string> GetOpenedPorts()
        {
            return dataCollector.GetOpenedPort();
        }


        public IEnumerable<int> GetDataAsInt(byte SlaveId, byte Function, ushort Addres, ushort Count, string portName)
        {
            var request = new ModbusRequest(SlaveId, Function, Addres, Count);



            return new int[3] { 2, 5, 6 };
        }

        public async Task<IEnumerable<bool>> GetDataAsBool(byte SlaveId, byte Function, ushort Addres, ushort Count, string portName)
        {
            var request = new ModbusRequest(SlaveId, Function, Addres, Count);

            return await dataCollector.GetAsBool(portName, request);
        }

        public IActionResult CreateCOMPort(string portName, int boundRate)
        {
            var portProps = new ComPortProps(portName, boundRate);

            if (dataCollector.AddNewComPort(portProps)) return Ok();

            return BadRequest();
        }

        public IActionResult CreateTCPPort(string ip, string port)
        {
            var portProps = new TCPPortProps(ip, port);

            if (dataCollector.AddNewTCPPort(portProps)) return Ok();

            return BadRequest();
        }

        public string DefaultPath()
        {
            var controller = RouteData.Values["controller"];
            var action = RouteData.Values["action"];
            return $"controller: {controller} | action: {action}";
        }

    }
}