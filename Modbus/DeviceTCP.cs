using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace ModbusGateWay.Modbus
{
    public class DeviceTCP : IModbus
    {
        private Socket _socket;
        private ILogger<DeviceTCP> _logger;
        readonly IPEndPoint ipPoint;

        public DeviceTCP(ILogger<DeviceCOMPort> logger, string ip, string port)
        {
            var _ip = IPAddress.Parse(ip);

            int _port = int.Parse(port);

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

        public int TreshBytes { get; private set; }

        public async Task<bool> OpenPort(string portName)
        {
            try
            {
                if (IsConnected) _socket.Close();

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                await _socket.ConnectAsync(ipPoint);

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

            TreshBytes += GetByte().Length;

            SendBytes += _socket.Send(request.Data);

            Console.WriteLine(DateTime.Now.ToString() + "-> Req:  " + request.ToString());
            Task.Delay(TimeOut).Wait();
            byte[] _read = GetByte();
            ReadBytes += _read.Length;
            ModbusMessage _out = new ModbusMessage(_read);
            Console.WriteLine(DateTime.Now.ToString() + "-> Resp: " + _out.ToString());

            if (_out.error != 0)
            {
                Errors++;
                return null;
            }

            if (_out.SlaveId != request.SlaveId) return null;

            PortBusy = false;
            return _out;
        }

        public int SendData(byte[] data)
        {
            try
            {
                _socket.Send(data);

                _logger.LogInformation($"Write {data.Length} bytes: {string.Join(" ", data)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendData", data);

                return 0;
            }

            return data.Length;
        }

        private byte[] GetByte()
        {
            byte[] _out;

            int availableBytes = _socket.Available;

            if (availableBytes > 0 && availableBytes < 256)
            {
                _out = new byte[availableBytes];

                _socket.Receive(_out, availableBytes, 0);

                return _out;
            }
            
            return new byte[0];
        }
    }
}
