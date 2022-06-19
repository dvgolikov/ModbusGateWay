using ModbusGateWay.Models;
using System.IO.Ports;

namespace ModbusGateWay.Modbus
{
    public class DeviceCOM : IModbus
    {
        private SerialPort _serialPort;
        private ILogger<DeviceCOM> _logger;

        public string LastExceptionText { get; private set; } = "";

        public DeviceCOM(ILogger<DeviceCOM> logger, ComPortProps portProps)
        {
            _logger = logger;

            _serialPort = new SerialPort();

            _serialPort.PortName = portProps.PortName;
            _serialPort.BaudRate = portProps.BaudRate;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
        }

        public bool OpenPort()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Port open Exception");

                LastExceptionText = ex.Message;

                return false;
            }

            return true;
        }

        public async Task<byte[]> ReceiveData()
        {
            int bytesToRead = _serialPort.BytesToRead;

            byte[] data = new byte[bytesToRead];

            if (data.Length == 0) return data;

            try
            {
                await _serialPort.BaseStream.ReadAsync(data, 0, bytesToRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReceiveData", data);

                LastExceptionText = ex.Message;
            }
            return data;
        }

        public async Task<bool> SendData(byte[] data)
        {
            try
            {
                await _serialPort.BaseStream.WriteAsync(data, 0, data.Length);

                _logger.LogInformation($"Write {data.Length} bytes: {string.Join(" ", data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendData", data);

                LastExceptionText = ex.Message;

                return false;
            }

            return true;
        }
    }
}
