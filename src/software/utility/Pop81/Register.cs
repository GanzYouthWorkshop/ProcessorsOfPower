using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace Pop81
{
    /// <summary>
    /// A POP81 processzor regiszterei.
    /// </summary>
    public enum RegisterCodes : byte
    {

        R1 = Register.GENERAL + Register.BITS8 + 0x00,
        R2 = Register.GENERAL + Register.BITS8 + 0x01,
        R3 = Register.GENERAL + Register.BITS8 + 0x02,
        R4 = Register.GENERAL + Register.BITS8 + 0x03,
        R5 = Register.GENERAL + Register.BITS8 + 0x04,
        R6 = Register.GENERAL + Register.BITS8 + 0x05,
        R7 = Register.GENERAL + Register.BITS8 + 0x06,
        R8 = Register.GENERAL + Register.BITS8 + 0x07,

        RA = Register.GENERAL + Register.BITS16 + 0x00,
        RB = Register.GENERAL + Register.BITS16 + 0x01,
        RC = Register.GENERAL + Register.BITS16 + 0x02,
        RD = Register.GENERAL + Register.BITS16 + 0x03,

        /// <summary>
        /// PC - Program counter.
        /// </summary>
        PC = Register.SPECIAL + Register.BITS16 + 0x00,

        /// <summary>
        /// DS - Data stack.
        /// </summary>
        DS = Register.SPECIAL + Register.BITS16 + 0x01,

        /// <summary>
        /// CS - Code stack.
        /// </summary>
        CS = Register.SPECIAL + Register.BITS16 + 0x02,

        /// <summary>
        /// MA - Memory address.
        /// </summary>
        MA = Register.SPECIAL + Register.BITS16 + 0x03,

        /// <summary>
        /// MD - Memory data.
        /// </summary>
        MD = Register.SPECIAL + Register.BITS8  + 0x04,

        /// <summary>
        /// FL - Flags.
        /// </summary>
        FL = Register.SPECIAL + Register.BITS8  + 0x05,
    }

    public class Register
    {
        /// <summary>
        /// Egy 8 bites regisztert jelöl.
        /// </summary>
        public const byte BITS8  = 0x00;

        /// <summary>
        /// Egy 16 bites regisztert jelöl.
        /// </summary>
        public const byte BITS16 = 0x80;

        /// <summary>
        /// Egy általános célú regisztert jelöl.
        /// </summary>
        public const byte GENERAL = 0x00;

        /// <summary>
        /// Egy specializált regisztert jelöl.
        /// </summary>
        public const byte SPECIAL = 0x40;

        private ulong m_InternalValue = 0;

        public byte B8
        {
            get { this.CheckSize(8); return (byte)this.m_InternalValue; }
            set { this.CheckSize(8); this.m_InternalValue = value; }
        }
        public ushort B16
        {
            get { this.CheckSize(16); return (ushort)this.m_InternalValue; }
            set { this.CheckSize(16); this.m_InternalValue = value; }
        }
        public uint B32
        {
            get { this.CheckSize(32); return (uint)this.m_InternalValue; }
            set { this.CheckSize(32); this.m_InternalValue = value; }
        }
        public ulong B64
        {
            get { this.CheckSize(64); return (ulong)this.m_InternalValue; }
            set { this.CheckSize(64); this.m_InternalValue = value; }
        }

        public object Auto
        {
            get
            {
                switch (SizeBits)
                {
                    case 8: return this.B8;
                    case 16: return this.B16;
                    case 32: return this.B32;
                    case 64: return this.B64;
                }
                return null;
            }
            set
            {
                switch (SizeBits)
                {
                    case 8: this.B8 = (byte)value; break;
                    case 16: this.B16 = (ushort)value; break;
                    case 32: this.B32 = (uint)value; break;
                    case 64: this.B64 = (ulong)value; break;
                }
            }
        }

        public int SizeBits { get; }

        public Register(int size = 8)
        {
            this.SizeBits = size;

            if (this.SizeBits != 8 && this.SizeBits != 16)
            {
                throw new ArgumentException("Not supported register size!");
            }
        }

        private void CheckSize(int size)
        {
            if (this.SizeBits != size)
            {
                throw new ArgumentException($"This is a {this.SizeBits}-bit register!");
            }
        }
    }
}
