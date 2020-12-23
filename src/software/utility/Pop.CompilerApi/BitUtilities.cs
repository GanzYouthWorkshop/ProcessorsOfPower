using System;
using System.Collections.Generic;
using System.Text;

namespace Pop.CompilerApi
{
    public class BitUtilities
    {
        public static byte[] GetBytes(ushort B16, ByteOrder endian)
        {
            byte[] result = BitConverter.GetBytes(B16);
            if(BitConverter.IsLittleEndian && endian == ByteOrder.BigEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static object ConvertToRaw<T>(byte[] bytes, int start)
        {
            Type t = typeof(T);
            object result = null;

            if (t.Equals(typeof(byte)))
            {
                result = bytes[start];
            }
            else if (t.Equals(typeof(ushort)))
            {
                result = BitConverter.ToUInt16(bytes, start);
            }
            else if (t.Equals(typeof(short)))
            {
                result = BitConverter.ToInt16(bytes, start);
            }
            //NOTE: továbbiakkal kéne bővíteni

            return result;
        }

        public static T ConvertTo<T>(byte[] bytes, int start)
        {
            object result = ConvertToRaw<T>(bytes, start);

            T typed = (T)Convert.ChangeType(result, typeof(T));
            return typed;
        }

        public static T ConvertTo<T>(byte[] bytes, int start, int size, bool signed)
        {
            object result = null;

            switch (size)
            {
                case 1:
                    result = ConvertToRaw<byte>(bytes, start);
                    break;
                case 2:
                    result = signed ? ConvertToRaw<short>(bytes, start) : ConvertToRaw<ushort>(bytes, start);
                    break;
            }

            T typed = (T)Convert.ChangeType(result, typeof(T));
            return typed;
        }

        public static byte[] ConvertFrom<T>(T source, Type overwriteType = null)
        {
            byte[] result = null;

            if(overwriteType == null)
            {
                overwriteType = typeof(T);
            }

            Type t = overwriteType;

            if (t.Equals(typeof(byte)))
            {
                result = new byte[] { (byte)Convert.ChangeType(source, typeof(byte)) };
            }
            else if (t.Equals(typeof(ushort)))
            {
                result = BitConverter.GetBytes((ushort)Convert.ChangeType(source, typeof(ushort)));
            }
            else if (t.Equals(typeof(short)))
            {
                result = BitConverter.GetBytes((short)Convert.ChangeType(source, typeof(short)));
            }

            return result;
        }
    }
}
