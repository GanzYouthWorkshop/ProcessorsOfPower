using System;
using System.Collections.Generic;
using System.Text;

namespace Pop81.Assembler
{
    public enum Mnemonics
    {
        NOPE,
        HALT,

        JUMP,
        JIFZ,
        JNOZ,
        JIFC,
        JNOC,
        JIFV,
        JNOV,
        JIFG,
        JNOG,

        LOAD,
        SAVE,
        
        POP,
        PUSH,

        CALL,
        RET,

        MOVE,

        COMP,

        ADD,
        SUB,
        MUL,
        DIV,
        INCR,
        DECR,

        AND,
        OR,
        NOT,
        LSFT,
        RSFT,
        LROT,
        RROT,
    }
}
