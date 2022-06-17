namespace ModbusGateWay.Models
{
    public class ModbusCommand : ModbusBaseRequest
    {
        public ushort Command { get; set; }
        public ModbusCommand(byte slaveId, byte function, ushort addres, ushort command = 0) : base(slaveId, function, addres)
        {
            Command = command;
        }
    }
}
