namespace Pop81
{
    /// <summary>
    /// A POP81 processzor által használt gépi nyelv opkódjainak listája.
    /// </summary>
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
        JumpIfNotZero_R,
        JumpIfNotZero_L,
        JumpIfCarry_R,
        JumpIfCarry_L,
        JumpIfNotCarry_R,
        JumpIfNotCarry_L,
        JumpIfGreater_R,
        JumpIfGreater_L,
        JumpIfNotGreater_R,
        JumpIfNotGreater_L,
        JumpIfParity_R,
        JumpIfParity_L,
        JumpIfNotParity_R,
        JumpIfNotParity_L,
        JumpIfSign_R,
        JumpIfSign_L,
        JumpIfNotSign_R,
        JumpIfNotSign_L,
        JumpIfHalfCarry_R,
        JumpIfHalfCarry_L,
        JumpIfNotHalfCarry_R,
        JumpIfNotHalfCarry_L,


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

        Compare_R,
        Compare_L,

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
