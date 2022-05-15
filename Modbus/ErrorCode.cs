namespace ModbusGateWay.Modbus
{
    [Flags]
    public enum ErrorCode
    {
        None = 0b_0000_0000,                // 0 - no errors
        NoData = 0b_0000_0001,              // 1 - message has no data
        DevError = 0b_0000_0010,            // 2 - Slave has not send any data
        CRCError = 0b_0000_0100,            // 4 - Message was broken while send
        ConvеrtDataError = 0b_0000_1000,    // 8 - Slave`s data has not recognize
    }
}
