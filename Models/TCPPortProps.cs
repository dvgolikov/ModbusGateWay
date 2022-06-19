namespace ModbusGateWay.Models
{
    public class TCPPortProps
    {
        public string Ip { get; }
        public string Port { get; }
        public string PortName => $"{Ip}:{Port}";
        public TCPPortProps(string ip, string port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
