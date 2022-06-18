namespace ModbusGateWay.Modbus
{
    public interface IModbus
    {
        int SendData(byte[] data);
        byte[] ReceiveData();
        bool OpenPort(string portName);
    }
}
