namespace Pop81
{
    public enum OpCode : byte
    {
        //halt
        //jumps
        //add, substract multiply, divide
        //add, or, not etc.
        //memory load, memory store
        //Stack -> pop/push
        //Interrupt handling
        //R<-R
        //R<-I

        /// <summary>
        /// No operation.
        /// </summary>
        Nop_X,

        /// <summary>
        /// Halt the CPU.
        /// </summary>
        Halt_X,

        Jump_R,
        Jump_L,
        JumpIfZero_R,
        JumpIfZero_L,
        JumpIfNotZero,
        JumpIfCarry,
        JumpIfNotCarry,

        Pop_X,
        Push_R,
        Push_L,

        Call_R,
        Call_L,
        Return_X,

        Load,
        Store,

        Move_R,
        Move_L,

        Add_R,
        Add_L,
        Substract_R,
        Substract_L,
        Multiply_R,
        Multiply_L,
        Divide_R,
        Divide_L,

        And_R,
        And_L,
        Or_R,
        Or_L,
        Not_R,   
        
        LShift_R,
        LShift_L,
        RShift_R,
        RShift_L,
        LRot_R,
        LRot_L,
        RRot_R,
        RRot_L,
    }
}
