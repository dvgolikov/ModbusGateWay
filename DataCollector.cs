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

        public ModbusMaster GetMaster(string portName)
        {
            if (!ModbusMasters.ContainsKey(portName))
            {
                var deviceCOMPort = new DeviceCOMPort(LoggerFactory.CreateLogger<DeviceCOMPort>());

                if (deviceCOMPort == null) return null;

                if (!deviceCOMPort.OpenPort(portName)) return null;

                var modbusMaster = new ModbusMaster(deviceCOMPort);

                if (modbusMaster == null) return null;

                ModbusMasters.Add(portName, modbusMaster);
            }

            return ModbusMasters[portName];
        }

        public DataCollector(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public IEnumerable<string> GetAvailableComPort()
        {
            return SerialPort.GetPortNames();
        }

        public async Task<IEnumerable<int>> GetAsInteger(string portName, ModbusRequest request)
        {
            var master = GetMaster(portName);

            var message = new ModbusMessage(request);

            var responce = await master.SendMessage(message);

            if (responce != null) return responce.GetAsInt();

            return new int[0];
        }

        public async Task<IEnumerable<float>> GetAsFloat(string portName, ModbusRequest request)
        {
            var master = GetMaster(portName);

            var message = new ModbusMessage(request);

            var responce = await master.SendMessage(message);

            if (responce != null) return responce.GetAsFloat();

            return new float[0];
        }

        public async Task<IEnumerable<bool>> GetAsBool(string portName, ModbusRequest request)
        {
            var master = GetMaster(portName);

            var message = new ModbusMessage(request);

            var responce = await master.SendMessage(message);

            if (responce != null) return responce.GetAsBool();

            return new bool[0];
        }

        public async Task<bool> ExecutCommand(string portName, ModbusCommand command)
        {
            var master = GetMaster(portName);

            var message = new ModbusMessage(command);

            var responce = await master.SendMessage(message);

            return responce == null ? false : responce.Error == 0;
        }
    }
}
