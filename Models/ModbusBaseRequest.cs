namespace ModbusGateWay.Models
{
    public class ModbusBaseRequest
    {
        public byte SlaveId { get; set; }
        public byte Function { get; set; }
        public ushort Addres { get; set; }

        public ModbusBaseRequest(byte slaveId, byte function, ushort addres)
        {
            SlaveId = slaveId;
            Function = function;
            Addres = addres;
        }
    }
}
