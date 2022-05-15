using ModbusGateWay.Modbus;

namespace ModbusGateWay.Services
{
    public class DataCaptureService : IHostedService
    {
        private readonly ModbusMessage _requestCurrent = new ModbusMessage(5, 0x3, 0xB00, 8);
        private readonly ModbusMessage _requestCounters = new ModbusMessage(5, 0x3, 0x218, 32);
        private readonly ModbusMessage _requestDI = new ModbusMessage(41, 0x1, 0x0, 16);

        private readonly ActualData _actualData;
        private readonly ILogger<DataCaptureService> _logger;
        private readonly ModbusMaster _modbusMaster;

        public DataCaptureService(ILogger<DataCaptureService> logger, ActualData actualData, IConfiguration configuration, IModbus deviceCOMPort)
        {
            _logger = logger;
            _actualData = actualData;

            string portName= configuration["PortName"];

            try
            {
                deviceCOMPort.OpenPort(portName);

                _modbusMaster = new ModbusMaster(deviceCOMPort);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Create port {portName}.");

                return;
            }



            Task.Factory.StartNew(async () =>
            {
                while(true)
                {
                    try
                    {
                        await UpdateCurrent();

                        await Task.Delay(2000);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Update data.");

                        break;
                    }
                }
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> UpdateCurrent()
        {
            var response = await _modbusMaster.SendMessage(_requestCurrent);

            _logger.LogInformation($"UpdateCurrent {response?.Error}");

            if (response == null)
            {
                _logger.LogError("No Responce");

                return false;
            }

            float[] f = response.FloatData;

            if (f.Length == 4)
            {
                _actualData.CurrentChanel1 = f[0];
                _actualData.CurrentChanel2 = f[1];
                _actualData.CurrentChanel3 = f[2];
                _actualData.CurrentChanel4 = f[3];
            }

            return true;
        }

        private async Task<bool> UpdateDI()
        {
            var response = await _modbusMaster.SendMessage(_requestCurrent);

            if (response == null) return false;

            bool[] responseBoolArray = response.BooleanData;

            if (responseBoolArray.Length != 16) return false;

            return true;
        }
    }
}
