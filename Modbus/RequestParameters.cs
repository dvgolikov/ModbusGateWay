namespace ModbusGateWay.Modbus
{
    public class RequestParameters
    {
        public byte SlaveId { get; set; }
        public byte Function { get; set; }
        public ushort Addres { get; set; }
        public ushort Count { get; set; }
        public ushort Command { get; set; }

        public RequestType Type = RequestType.AsByte;
        public RequestParameters(byte slaveId, byte function, UInt16 addres, UInt16 count = 0, UInt16 command = 0)
        {
            SlaveId = slaveId;
            Function = function;
            Addres = addres;
            Count = count;
            Command = command;
        }
    }
}
