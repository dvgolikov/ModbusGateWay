namespace ModbusGateWay.Models
{
    public class ComPortProps
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }

        public ComPortProps(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
        }
    }
}
