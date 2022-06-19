using ModbusGateWay.Models;
using System.Text;

namespace ModbusGateWay.Modbus
{
    public class ModbusMessage
    {
        private readonly byte[] _data;
        public string ExceptionText { get; } = "";

        public ErrorCode Error { get; private set; } = ErrorCode.None;
        public byte SlaveId => _data[0];
        public byte Function => _data[1];
        public byte Count => _data[2];
        public byte[] Data => _data;

        public ModbusMessage(ModbusRequest request)
        {
            _data = new byte[8];
            _data[0] = request.SlaveId;
            _data[1] = request.Function;
            _data[2] = BitConverter.GetBytes(request.Addres)[1];
            _data[3] = BitConverter.GetBytes(request.Addres)[0];
            _data[4] = BitConverter.GetBytes(request.Count)[1];
            _data[5] = BitConverter.GetBytes(request.Count)[0];

            byte[] _temp = BitConverter.GetBytes(CalcCRC(_data, 6));

            _data[6] = _temp[0];
            _data[7] = _temp[1];
        }

        public ModbusMessage(ModbusCommand command)
        {
            _data = new byte[8];
            _data[0] = command.SlaveId;
            _data[1] = command.Function;
            _data[2] = BitConverter.GetBytes(command.Addres)[1];
            _data[3] = BitConverter.GetBytes(command.Addres)[0];
            _data[4] = BitConverter.GetBytes(command.Command)[1];
            _data[5] = BitConverter.GetBytes(command.Command)[0];

            byte[] _temp = BitConverter.GetBytes(CalcCRC(_data, 6));

            _data[6] = _temp[0];
            _data[7] = _temp[1];
        }

        public ModbusMessage(byte[] data)
        {
            _data = data;

            if (_data.Length < 3)
            {
                Error |= ErrorCode.NoData;
            }

            if (CheckCRC(_data)) Error |= ErrorCode.CRCError;

            if (SlaveId > 64) Error |= ErrorCode.DevError;
        }

        public ModbusMessage(string exceptionText)
        {
            ExceptionText = exceptionText;
            _data = new byte[0];
        }

        private static UInt16 CalcCRC(byte[] _in, int num)
        {
            UInt16 crc16 = 0xffff;

            for (int i = 0; i < num; i++)
            {
                crc16 = (UInt16)(crc16 ^ _in[i]);

                for (UInt16 c = 8; c > 0; c--)
                {
                    if ((UInt16)(crc16 & 0x01) != 0)
                    {
                        crc16 = (UInt16)(crc16 >> 1);
                        crc16 = (UInt16)(crc16 ^ 0x0a001);
                    }
                    else crc16 = (UInt16)(crc16 >> 1);
                }
            }

            return crc16;
        }

        private static bool CheckCRC(byte[] _in)
        {
            if (_in.Length < 3) return false;

            byte[] _temp = BitConverter.GetBytes(CalcCRC(_in, _in.Length - 2));

            return (_temp[0] == _in[_in.Length - 1] && _temp[1] == _in[_in.Length - 2]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte temp in _data)
                sb.Append(temp.ToString("X") + " ");

            return sb.ToString();
        }

        public IEnumerable<float> GetAsFloat()
        {
            int floatCount = Count / 4;

            if (floatCount < 1 || Count % 4 != 0) Error |= ErrorCode.ConvеrtDataError;

            if (Error != 0) return new float[0];

            float[] _out = new float[floatCount];

            for (int i = 0; i < floatCount; i++)
            {
                _out[i] = System.BitConverter.ToSingle(
                    new byte[]{
                            _data[6 + i * 4],
                            _data[5 + i * 4],
                            _data[4 + i * 4],
                            _data[3 + i * 4]
                    }, 0);
            }

            return _out;

        }

        public IEnumerable<bool> GetAsBool()
        {
                if (Error != 0) return new bool[0];

                bool[] _return = new bool[Count * 8];

                for (int j = 0; j < Count; j++)
                    for (int i = 0; i < 8; i++)
                        _return[j * 8 + i] = (_data[3 + j] & (1 << i)) != 0;

                return _return;
        }

        public IEnumerable<int> GetAsInt()
        {
                int intCount = (int)(Count / 2);

                if (intCount < 1 || Count % 2 != 0) Error |= ErrorCode.ConvеrtDataError;

                if (Error != 0) return new int[0];

                int[] _out = new int[intCount];

                for (int j = 0; j < intCount; j++)
                {
                    _out[j] = (_data[5 + j * 2]) * 256 + _data[6 + j * 2];
                }

                return _out;
        }
    }
}
