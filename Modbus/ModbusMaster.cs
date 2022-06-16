namespace ModbusGateWay.Modbus
{
    public class ModbusMaster
    {
        private IModbus _modbus;
        private SemaphoreSlim _semaphore;

        public int SendBytes { get; private set; }
        public int ReadBytes { get; private set; }
        public int SendReq { get; private set; }
        public int ReadAns { get; private set; }
        public int Errors { get; private set; }
        public int TreshBytes { get; private set; }


        public async Task<ModbusMessage?> SendMessage(ModbusMessage request)
        {
            await _semaphore.WaitAsync();

            try
            {
                var treshData = _modbus.ReceiveData();

                TreshBytes += treshData.Length;

                SendBytes += _modbus.SendData(request.Data);

                await Task.Delay(100);

                byte[] _read = _modbus.ReceiveData();

                ReadBytes += _read.Length;

                ModbusMessage _out = new ModbusMessage(_read);

                return _out;
            }
            catch(Exception ex)
            {
                return null;

                _semaphore.Release();
            }
        }


        public ModbusMaster(IModbus modbus)
        {
            _modbus = modbus;

            _semaphore = new SemaphoreSlim(1);
        }
    }
}
