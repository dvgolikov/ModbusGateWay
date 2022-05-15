using System.Text;

namespace ModbusGateWay.Modbus
{
    public class ModbusMessage
    {
        public byte SlaveId { get; private set; }

        public byte Function { get; private set; }

        public UInt16 Count { get; private set; }

        public UInt16 Command { get; private set; }

        public UInt16 Addres { get; private set; }

        public ErrorCode Error { get; private set; } = ErrorCode.None;

        private byte[] _data;

        public byte[] Data
        {
            get
            {
                _data = new byte[8];
                _data[0] = SlaveId;
                _data[1] = Function;
                _data[2] = BitConverter.GetBytes(Addres)[1];
                _data[3] = BitConverter.GetBytes(Addres)[0];

                if (Count > 0 && Command == 0)
                {
                    _data[4] = BitConverter.GetBytes(Count)[1];
                    _data[5] = BitConverter.GetBytes(Count)[0];
                }

                if (Count == 0 && Command > 0)
                {
                    _data[4] = BitConverter.GetBytes(Command)[1];
                    _data[5] = BitConverter.GetBytes(Command)[0];
                }

                byte[] _temp = BitConverter.GetBytes(CalcCRC(_data, 6));

                _data[6] = _temp[0];
                _data[7] = _temp[1];

                return _data;
            }
            set
            {
                _data = value;

                if (_data.Length < 3)
                {
                    Error |= ErrorCode.NoData;
                    return;
                }

                //if (CheckCRC(_data)) Error |= ErrorCode.CRCError;

                if (_data[0] > 64) Error |= ErrorCode.DevError;

                if (Error == 0)
                {
                    SlaveId = _data[0];
                    Function = _data[1];
                    Count = _data[2];
                }
            }
        }

        public ModbusMessage(byte slaveId, byte function, UInt16 addres, UInt16 count = 0, UInt16 command = 0)
        {
            SlaveId = slaveId;
            Function = function;
            Addres = addres;
            Count = count;
            Command = command;
        }

        public ModbusMessage(byte[] data) => Data = data;

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

            return (crc16);
        }

        private static bool CheckCRC(byte[] _in)
        {
            if (_in.Length < 3) return false;

            byte[] _temp = BitConverter.GetBytes(CalcCRC(_in, _in.Length - 2));

            return (_temp[0] == _in[_in.Length - 2] && _temp[1] == _in[_in.Length - 1]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte temp in _data)
                sb.Append(temp.ToString("X") + " ");

            return sb.ToString();
        }

        public float[] FloatData
        {
            get
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
        }

        public bool[] BooleanData
        {
            get
            {
                if (Error != 0) return new bool[0];

                bool[] _return = new bool[Count * 8];

                for (int j = 0; j < Count; j++)
                    for (int i = 0; i < 8; i++)
                        _return[j * 8 + i] = (_data[3 + j] & (1 << i)) != 0;

                return _return;
            }
        }

        public int[] IntegerData
        {
            get
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
}
