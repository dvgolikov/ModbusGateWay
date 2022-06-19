using ModbusGateWay.Modbus;
using ModbusGateWay.Models;
using System.IO.Ports;

namespace ModbusGateWay
{
    public class DataCollector
    {
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration Configuration { get; }

        private Dictionary<string, ModbusMaster> ModbusMasters { get; }

        private readonly ILogger<DataCollector> _log;

        public string AddNewComPort(ComPortProps portProps)
        {
            var deviceCOMPort = new DeviceCOM(LoggerFactory.CreateLogger<DeviceCOM>(), portProps);

            if (deviceCOMPort == null) return "Can't create Serial port.";

            if (!deviceCOMPort.OpenPort()) return deviceCOMPort.LastExceptionText;

            var modbusMaster = new ModbusMaster(deviceCOMPort);

            ModbusMasters.Add(portProps.PortName, modbusMaster);

            return "";
        }

        public bool AddNewTCPPort(TCPPortProps portProps)
        {
            var deviceCOMPort = new DeviceTCP(LoggerFactory.CreateLogger<DeviceTCP>(), portProps);

            if (deviceCOMPort == null) return false;

            if (!deviceCOMPort.OpenPort()) return false;

            var modbusMaster = new ModbusMaster(deviceCOMPort);

            if (modbusMaster == null) return false;

            ModbusMasters.Add(portProps.PortName, modbusMaster);

            return true;
        }

        public DataCollector(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;

            ModbusMasters = new Dictionary<string, ModbusMaster>();

            _log = LoggerFactory.CreateLogger<DataCollector>();
        }

        public IEnumerable<string> GetAvailableComPort()
        {
            return SerialPort.GetPortNames();
        }

        public IEnumerable<string> GetOpenedPort()
        {
            return ModbusMasters.Keys;
        }

        public async Task<IEnumerable<int>> GetAsInteger(string portName, ModbusRequest request)
        {
            var master = ModbusMasters[portName];

            var message = new ModbusMessage(request);

            var responce = await master.SendMessage(message);

            if (responce != null) return responce.GetAsInt();

            return new int[0];
        }

        public async Task<IEnumerable<float>> GetAsFloat(string portName, ModbusRequest request)
        {
            var master = ModbusMasters[portName];

            var message = new ModbusMessage(request);

            var responce = await master.SendMessage(message);

            if (responce != null) return responce.GetAsFloat();

            return new float[0];
        }

        public async Task<IEnumerable<bool>> GetAsBool(string portName, ModbusRequest request)
        {
            var master = ModbusMasters[portName];

            var message = new ModbusMessage(request);

            _log.LogInformation(message.ToString());

            var responce = await master.SendMessage(message);

            _log.LogInformation(responce?.ToString());

            if (responce != null) return responce.GetAsBool();

            return new bool[0];
        }

        public async Task<bool> ExecutCommand(string portName, ModbusCommand command)
        {
            var master = ModbusMasters[portName];

            var message = new ModbusMessage(command);

            var responce = await master.SendMessage(message);

            return responce == null ? false : responce.Error == 0;
        }
    }
}
