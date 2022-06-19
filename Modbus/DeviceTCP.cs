using ModbusGateWay.Models;
using System.Net;
using System.Net.Sockets;

namespace ModbusGateWay.Modbus
{
    public class DeviceTCP : IModbus
    {
        private readonly Socket _socket;
        private readonly ILogger<DeviceTCP> _logger;
        private readonly IPEndPoint _ipPoint;

        public string LastExceptionText { get; private set; } = "";

        public DeviceTCP(ILogger<DeviceTCP> logger, TCPPortProps portProps)
        {
            _logger = logger;

            var _ip = IPAddress.Parse(portProps.Ip);

            int _port = int.Parse(portProps.Port);

            _ipPoint = new IPEndPoint(_ip, _port);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool OpenPort()
        {
            try
            {
                if (_socket.Connected) _socket.Close();

                _socket.Connect(_ipPoint);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Port open Exception");

                LastExceptionText=ex.Message;

                return false;
            }           
        }

        public async Task<byte[]> ReceiveData()
        {
            byte[] _out;

            int availableBytes = _socket.Available;

            if (availableBytes > 0 && availableBytes < 256)
            {
                _out = new byte[availableBytes];

                await _socket.ReceiveAsync(_out, 0);

                return _out;
            }

            return new byte[0];
        }

        public async Task<bool> SendData(byte[] data)
        {
            try
            {
                await _socket.SendAsync(data, 0);

                _logger.LogInformation($"Write {data.Length} bytes: {string.Join(" ", data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendData", data);

                return false;
            }

            return true;
        }
    }
}
