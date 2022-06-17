using ModbusGateWay.Modbus;
using ModbusGateWay.Models;
using System.IO.Ports;

namespace ModbusGateWay
{
    public class DataCollector
    {
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration Configuration { get; }

        public Dictionary<string, ModbusMaster> ModbusMasters { get; }

        public DataCollector(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public IEnumerable<string> GetAvailableComPort()
        {
            return SerialPort.GetPortNames();
        }

        public GetAsInteger()


        public bool ExecutCommand(string portName, ModbusCommand modbusCommand)
        {
            if(!ModbusMasters.ContainsKey(portName))
            {
                var deviceCOMPort = new DeviceCOMPort(LoggerFactory.CreateLogger<DeviceCOMPort>());

                if(deviceCOMPort==null) return false;

                if (!deviceCOMPort.OpenPort(portName)) return false;

                var modbusMaster = new ModbusMaster(deviceCOMPort);

                if (modbusMaster == null) return false;

                ModbusMasters.Add(portName, modbusMaster);
            }

            ModbusMasters[portName].SendMessage(modbusCommand)
        }
    }
}
