namespace ModbusGateWay.Modbus
{
    public interface IModbus
    {
        public string LastExceptionText { get; }
        Task<bool> SendData(byte[] data);
        Task<byte[]> ReceiveData();
        bool OpenPort();
    }
}
