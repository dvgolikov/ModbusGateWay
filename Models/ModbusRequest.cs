namespace ModbusGateWay.Models
{
    public class ModbusRequest : ModbusBaseRequest
    {
        public ushort Count { get; set; }
        public ModbusRequest(byte slaveId, byte function, ushort addres, ushort count) : base(slaveId, function, addres)
        {
            Count=count;
        }
    }
}
