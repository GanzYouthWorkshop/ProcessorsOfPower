using System;
using System.Collections.Generic;
using System.Text;

namespace Pop81
{
    [Flags]
    public enum AluFlags : byte
    {
        Zero = 1,
        Carry = 2,
        Sign = 4,
        Parity = 8,
        HalfCarry = 16,
        Greater = 32
    }
}
