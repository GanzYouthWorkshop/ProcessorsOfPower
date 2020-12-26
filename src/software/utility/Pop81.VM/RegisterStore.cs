using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pop.CompilerApi;
using System.Runtime.InteropServices;

namespace Pop81.VM
{
    public class RegisterStore<T>
    {
        public class RegisterDescriptor
        {
            public T ID { get; set; }
            public int Address { get; set; }
            public int SizeInBytes { get; set; }

            public RegisterDescriptor(T id, int  address, int size)
            {
                this.ID = id;
                this.Address = address;
                this.SizeInBytes = size;
            }
        }

        public class RegisterAccessor<K>
        {
            public RegisterStore<T> ParentStore { get; set; }

            public static readonly int SizeOfK;

            static RegisterAccessor()
            {
                SizeOfK = Marshal.SizeOf<K>();
            }

            public K this[T register]
            {
                get
                {
                    var descriptor = this.ParentStore.Registers.FirstOrDefault(desc => desc.ID.Equals(register));
                    if (descriptor != null)
                    {
                        return BitUtilities.ConvertTo<K>(this.ParentStore.m_InnerData, descriptor.Address, descriptor.SizeInBytes, false);
                    }
                    throw new Exception("Register does not exist!");
                }

                set
                {
                    var descriptor = this.ParentStore.Registers.FirstOrDefault(desc => desc.ID.Equals(register));
                    if (descriptor != null)
                    {
                        byte[] bytes = BitUtilities.ConvertFrom<K>(value);
                        Array.Copy(bytes, 0, this.ParentStore.m_InnerData, descriptor.Address, bytes.Length);
                        return;
                    }
                    throw new Exception("Register does not exist!");
                }
            }
        }

        protected byte[] m_InnerData;

        public List<RegisterDescriptor> Registers { get; set; }
    }

    public class Pop81Registers : RegisterStore<RegisterCodes>
    {
        public RegisterAccessor<byte> B8 { get; private set; }
        public RegisterAccessor<ushort> B16 { get; private set; }

        public Pop81Registers()
        {
            this.B8 = new RegisterAccessor<byte>() { ParentStore = this };
            this.B16 = new RegisterAccessor<ushort>() { ParentStore = this };

            this.Registers = new List<RegisterDescriptor>()
            {
                new RegisterDescriptor(RegisterCodes.R1, 0, 1),
                new RegisterDescriptor(RegisterCodes.R2, 1, 1),
                new RegisterDescriptor(RegisterCodes.R3, 2, 1),
                new RegisterDescriptor(RegisterCodes.R4, 3, 1),
                new RegisterDescriptor(RegisterCodes.R5, 4, 1),
                new RegisterDescriptor(RegisterCodes.R6, 5, 1),
                new RegisterDescriptor(RegisterCodes.R7, 6, 1),
                new RegisterDescriptor(RegisterCodes.R8, 7, 1),

                new RegisterDescriptor(RegisterCodes.RA, 0, 2),
                new RegisterDescriptor(RegisterCodes.RB, 2, 2),
                new RegisterDescriptor(RegisterCodes.RC, 4, 2),
                new RegisterDescriptor(RegisterCodes.RD, 6, 2),

                new RegisterDescriptor(RegisterCodes.PC, 8, 2),
                new RegisterDescriptor(RegisterCodes.DS, 10, 2),
                new RegisterDescriptor(RegisterCodes.CS, 12, 2),
                new RegisterDescriptor(RegisterCodes.MA, 14, 2),
                new RegisterDescriptor(RegisterCodes.MD, 16, 1),
                new RegisterDescriptor(RegisterCodes.FL, 17, 1),
            };

            this.m_InnerData = new byte[18];
        }

        public void SetFlag(AluFlags flag, bool value)
        {
            byte mask = (byte)flag;
            if(!value)
            {
                mask = (byte)~(int)mask;
                this.B8[RegisterCodes.FL] = (byte)((int)this.B8[RegisterCodes.FL] & (int)mask);
            }
            else
            {
                this.B8[RegisterCodes.FL] = (byte)((int)this.B8[RegisterCodes.FL] | (int)mask);
            }
        }

        public bool GetFlag(AluFlags flag)
        {
            return ((AluFlags)this.B8[RegisterCodes.FL]).HasFlag(flag);
        }
    }
}
