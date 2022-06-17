using System.IO.Ports;

namespace ModbusGateWay.Modbus
{
    public class DeviceCOMPort : IModbus
    {
        private SerialPort _serialPort;
        private ILogger<DeviceCOMPort> _logger;

        public DeviceCOMPort(ILogger<DeviceCOMPort> logger)
        {
            _logger = logger;
        }

        public bool OpenPort(string portName)
        {
            try
            {
                _serialPort = new SerialPort();

                _serialPort.PortName = portName;
                _serialPort.BaudRate = 115200;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;

                _serialPort.Open();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Port open Exception");

                return false;
            }

            return true;
        }

        public byte[] ReceiveData()
        {
            int bytesToRead = _serialPort.BytesToRead;

            byte[] data = new byte[bytesToRead];

            try
            {
                _serialPort.Read(data, 0, bytesToRead);

                _logger.LogInformation($"Read {bytesToRead} bytes: {string.Join(" ", data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReceiveData", data);

                return data;
            }
            return data;
        }

        public int SendData(byte[] data)
        {
            try
            {
                _serialPort.Write(data, 0, data.Length);

                _logger.LogInformation($"Write {data.Length} bytes: {string.Join(" ", data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendData", data);

                return 0;
            }

            return data.Length;
        }
    }
}
