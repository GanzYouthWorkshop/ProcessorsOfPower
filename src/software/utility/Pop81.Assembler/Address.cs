using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pop81.Assembler
{
    public class Address
    {
        public enum Types
        {
            Register,
            Literal,
            Indirect
        }

        public Types Type { get; set; }
        public int Length { get; set; }

        public byte B8 { get; set; }
        public ushort B16 { get; set; }
        
        public Address()
        {

        }

        public Address(Types type, int size, ushort value)
        {
            this.Type = type;
            this.Length = size;
            if(Length == 8)
            {
                this.B8 = (byte)value;
            }
            else
            {
                this.B16 = value;
            }
        }

        public static Address FromString(string address)
        {
            Address result = new Address();

            IEnumerable<string> haystack = Enum.GetNames(typeof(RegisterCodes)).Select(s => s.ToUpper());

            if(haystack.Contains(address.ToUpper()))
            {
                result.Type = Types.Register;
                result.Length = 8;
                result.B8 = (byte)Enum.Parse(typeof(RegisterCodes), address.ToUpper());
            }
            else if(address.StartsWith('['))
            {
                //todo
                result.Type = Types.Indirect;
            }
            else
            {
                result.Type = Types.Literal;
                result.Length = 16;
                result.B16 = AssemblerOld.LiteralToShort(address);
            }

            return result;
        }
    }
}
