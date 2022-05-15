namespace ModbusGateWay.Modbus
{
    public interface IModbus
    {
        int SendData(byte[] data);
        byte[] ReceiveData();
        void OpenPort(string portName);
    }
}
