namespace ModbusGateWay.Modbus
{
    public class ModbusMaster
    {
        private readonly IModbus _modbus;
        private readonly SemaphoreSlim _semaphore;

        public int SendBytes { get; private set; }
        public int ReadBytes { get; private set; }
        public int SendReq { get; private set; }
        public int ReadAns { get; private set; }
        public int Errors { get; private set; }
        public int TreshBytes { get; private set; }

        public async Task<ModbusMessage> SendMessage(ModbusMessage request)
        {
            await _semaphore.WaitAsync();

            try
            {
                var treshData = await _modbus.ReceiveData();

                TreshBytes += treshData.Length;

                if (await _modbus.SendData(request.Data)) SendBytes += request.Data.Length;
                else return new ModbusMessage(_modbus.LastExceptionText);

                await Task.Delay(100);

                byte[] _data = await _modbus.ReceiveData();

                if(_data.Length==0) return new ModbusMessage(_modbus.LastExceptionText);

                ReadBytes += _data.Length;

                return new ModbusMessage(_data);
            }
            catch (Exception ex)
            {
                return new ModbusMessage(ex.Message);
            }
            finally
            {
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
