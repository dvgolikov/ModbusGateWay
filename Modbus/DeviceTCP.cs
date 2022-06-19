using ModbusGateWay.Models;
using System.Net;
using System.Net.Sockets;

namespace ModbusGateWay.Modbus
{
    public class DeviceTCP : IModbus
    {
        private Socket _socket;
        private ILogger<DeviceTCP> _logger;
        readonly IPEndPoint ipPoint;

        public string LastExceptionText { get; private set; }

        public DeviceTCP(ILogger<DeviceTCP> logger, TCPPortProps portProps)
        {
            var _ip = IPAddress.Parse(portProps.Ip);

            int _port = int.Parse(portProps.Port);

            var ipPoint = new IPEndPoint(_ip, _port);
        }

        public bool IsConnected
        {
            get
            {
                if (_socket == null) return false;
                return _socket.Connected;
            }
        }

        public bool OpenPort()
        {
            try
            {
                if (IsConnected) _socket.Close();

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _socket.Connect(ipPoint);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Port open Exception");

                return false;
            }

            return true;
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
