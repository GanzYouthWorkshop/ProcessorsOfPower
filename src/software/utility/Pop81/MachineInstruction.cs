using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Pop81
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MachineInstruction
    {
        [FieldOffset(0)] public uint Data;

        [FieldOffset(0)] public byte OpcodeByte;
        [FieldOffset(0)] public OpCode Opcode;

        [FieldOffset(1)] public byte Target;
        [FieldOffset(1)] public byte TargetRegister;

        [FieldOffset(2)] public byte Source;
        [FieldOffset(1)] public byte SourceRegister;
        [FieldOffset(2)] public ushort Literal;

        [FieldOffset(0)] public byte Byte1;
        [FieldOffset(1)] public byte Byte2;
        [FieldOffset(2)] public byte Byte3;
        [FieldOffset(3)] public byte Byte4;

        public MachineInstruction(byte[] bytes)
        {
            this.Data = 0;
            this.Opcode = 0;
            this.OpcodeByte = 0;
            this.Target = 0;
            this.TargetRegister = 0;
            this.Source = 0;
            this.SourceRegister = 0;
            this.Literal = 0;
            this.Byte1 = 0;
            this.Byte2 = 0;
            this.Byte3 = 0;
            this.Byte4 = 0;

            if (bytes != null && bytes.Length == 4)
            {
                this.Byte1 = bytes[0];
                this.Byte2 = bytes[1];
                this.Byte3 = bytes[2];
                this.Byte4 = bytes[3];
            }
            else
            {
                throw new ArgumentException("Bytes cannot be null and needs and exact length!");
            }
        }

        public string FormatToDebug()
        {
            int ci = Utils.INSTR_COLOR;
            int ct = Utils.TO_REG_COLOR;
            int cs = Utils.FROM_REG_COLOR;
            string b1 = this.Byte1.ToString("x2");
            string b2 = this.Byte2.ToString("x2");
            string b3 = this.Byte3.ToString("x2");
            string b4 = this.Byte4.ToString("x2");
            return $"\u001b[38;5;{ci}m{b1}\u001b[0m-\u001b[38;5;{ct}m{b2}\u001b[0m-\u001b[38;5;{cs}m{b3}\u001b[0m-\u001b[38;5;{cs}m{b4}\u001b[0m";
        }
    }
}
