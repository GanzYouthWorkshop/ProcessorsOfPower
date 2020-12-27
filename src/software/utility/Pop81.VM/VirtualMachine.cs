using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Pop81.VM.Implementation
{
    public class VirtualMachine
    {
        public enum RunMode
        {
            RealTime,
            Slow,
            HandTicked
        }

        public byte[] MainMemory { get; } = new byte[32 * 1024];

        public Pop81Registers Registers { get; } = new Pop81Registers();

        public RunMode CurrentMode { get; set; } = RunMode.HandTicked;
        public AutoResetEvent Ticker { get; } = new AutoResetEvent(false);

        public bool CanRun { get; set; } = true;

        public VirtualMachine()
        {
        }

        public void Start()
        {
            new Thread(new ThreadStart(this.Runner))
            {
                Name = "[/] Main VM thread",
                IsBackground = true
            }.Start();
        }

        public void SetupROM(byte[] contents)
        {
            contents.CopyTo(this.MainMemory, 0);
        }

        public void Runner()
        {
            try
            {
                while(this.CanRun)
                {
                    if (this.CurrentMode == RunMode.HandTicked)
                    {
                        this.Ticker.WaitOne();
                    }

                    ushort pp = this.Registers.B16[RegisterCodes.PC];
                    byte[] m = this.MainMemory;

                    MachineInstruction instruction = new MachineInstruction(new byte[] { m[pp++], m[pp++], m[pp++], m[pp++] });
                    this.ExecuteInstruction(instruction);

                    ushort temp = this.Registers.B16[RegisterCodes.PC];
                    if (pp - temp == 4)
                    {
                        //Erre azért van szükség, hogy ha esetleg ugró utasítást használtunk volna akkor ne írjuk felül az ugrási címet!
                        this.Registers.B16[RegisterCodes.PC] = pp;
                    }

                    switch (this.CurrentMode)
                    {
                        case RunMode.RealTime: break; //Ne cvárakozz semmire, ami a csövön kifér!
                        case RunMode.Slow: Thread.Sleep(200); break;
                        case RunMode.HandTicked: this.Ticker.Reset(); break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something went terribly wrong!");
            }
        }

        public void ExecuteInstruction(MachineInstruction instruction)
        {
            switch(instruction.Opcode)
            {
                #region NOP
                case OpCode.Nop_X:
                    return;
                #endregion
                #region HALT
                case OpCode.Halt_X:
                    this.CanRun = false; break;
                #endregion

                #region JUMP
                case OpCode.Jump_R:
                    this.Registers.B16[RegisterCodes.PC] = this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Jump_L:
                    this.Registers.B16[RegisterCodes.PC] = instruction.Literal;
                    break;
                #endregion
                #region JIFZ
                case OpCode.JumpIfZero_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Zero, true);
                    break;
                case OpCode.JumpIfZero_L:
                    this.DoJump(instruction.Literal, AluFlags.Zero, true);
                    break;
                #endregion
                #region JINZ
                case OpCode.JumpIfNotZero_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Zero, false);
                    break;
                case OpCode.JumpIfNotZero_L:
                    this.DoJump(instruction.Literal, AluFlags.Zero, false);
                    break;
                #endregion
                #region JIFG
                case OpCode.JumpIfGreater_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Greater, true);
                    break;
                case OpCode.JumpIfGreater_L:
                    this.DoJump(instruction.Literal, AluFlags.Greater, true);
                    break;
                #endregion
                #region JING
                case OpCode.JumpIfNotGreater_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Greater, false);
                    break;
                case OpCode.JumpIfNotGreater_L:
                    this.DoJump(instruction.Literal, AluFlags.Greater, false);
                    break;
                #endregion
                #region JIFC
                case OpCode.JumpIfCarry_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Carry, true);
                    break;
                case OpCode.JumpIfCarry_L:
                    this.DoJump(instruction.Literal, AluFlags.Carry, true);
                    break;
                #endregion
                #region JINC
                case OpCode.JumpIfNotCarry_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Carry, false);
                    break;
                case OpCode.JumpIfNotCarry_L:
                    this.DoJump(instruction.Literal, AluFlags.Carry, false);
                    break;
                #endregion
                #region JIFP
                case OpCode.JumpIfParity_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Parity, true);
                    break;
                case OpCode.JumpIfParity_L:
                    this.DoJump(instruction.Literal, AluFlags.Parity, true);
                    break;
                #endregion
                #region JINP
                case OpCode.JumpIfNotParity_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Parity, false);
                    break;
                case OpCode.JumpIfNotParity_L:
                    this.DoJump(instruction.Literal, AluFlags.Parity, false);
                    break;
                #endregion
                #region JIFS
                case OpCode.JumpIfSign_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Sign, true);
                    break;
                case OpCode.JumpIfSign_L:
                    this.DoJump(instruction.Literal, AluFlags.Sign, true);
                    break;
                #endregion
                #region JINS
                case OpCode.JumpIfNotSign_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.Sign, false);
                    break;
                case OpCode.JumpIfNotSign_L:
                    this.DoJump(instruction.Literal, AluFlags.Sign, false);
                    break;
                #endregion
                #region JIFH
                case OpCode.JumpIfHalfCarry_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.HalfCarry, true);
                    break;
                case OpCode.JumpIfHalfCarry_L:
                    this.DoJump(instruction.Literal, AluFlags.HalfCarry, true);
                    break;
                #endregion
                #region JINH
                case OpCode.JumpIfNotHalfCarry_R:
                    this.DoJump(instruction.TargetRegister, AluFlags.HalfCarry, false);
                    break;
                case OpCode.JumpIfNotHalfCarry_L:
                    this.DoJump(instruction.Literal, AluFlags.HalfCarry, false);
                    break;
                #endregion

                #region COMP
                case OpCode.Compare_R:
                    ushort a = this.Registers.B16[instruction.TargetRegister];
                    ushort b = this.Registers.B16[instruction.SourceRegister];

                    this.Registers.SetFlag(AluFlags.Zero, a == b);
                    break;
                case OpCode.Compare_L:
                    a = this.Registers.B16[instruction.TargetRegister];
                    b = instruction.Literal;

                    this.Registers.SetFlag(AluFlags.Zero, a == b);
                    break;
                #endregion

                //case OpCode.Pop:
                //    this.Registers[instruction.TargetRegister].B8 = this.MainMemory[this.Registers[RegisterCodes.DSI].B16];
                //    this.Registers[RegisterCodes.DSI].B16--;
                //    break;
                //case OpCode.Push:
                //    this.Registers[RegisterCodes.DSI].B16++;
                //    this.MainMemory[this.Registers[RegisterCodes.DSI].B16] = this.Registers[instruction.TargetRegister].B8;
                //    break;


                //case OpCode.call:
                //    this.Registers[instruction.TargetRegister].B8 = this.MainMemory[this.Registers[RegisterCodes.DSI].B16];
                //    this.Registers[RegisterCodes.DSI].B16--;
                //    break;
                //case OpCode.Return_X:
                //    this.Registers[RegisterCodes.DSI].B16++;
                //    this.MainMemory[this.Registers[RegisterCodes.DSI].B16] = this.Registers[instruction.TargetRegister].B8;
                //    break;

                //case OpCode.Load:
                //    this.Registers[RegisterCodes.MD].B8 = this.MainMemory[this.Registers[RegisterCodes.MA].B16];
                //    break;
                //case OpCode.Store:
                //    this.MainMemory[this.Registers[RegisterCodes.MA].B16] = this.Registers[RegisterCodes.MD].B8;
                //    break;

                case OpCode.Store:
                    this.MainMemory[this.Registers.B16[RegisterCodes.MA]] = this.Registers.B8[RegisterCodes.MD];
                    break;
                case OpCode.Load:
                     this.Registers.B8[RegisterCodes.MD] = this.MainMemory[this.Registers.B16[RegisterCodes.MA]];
                    break;

                #region MOVE
                case OpCode.Move_R:
                    this.Registers.B16[instruction.TargetRegister] = this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Move_L:
                    this.Registers.B16[instruction.TargetRegister] = instruction.Literal;
                    break;
                #endregion

                #region ADD
                case OpCode.Add_R:
                    this.ExecuteArithmeticOperation(instruction.TargetRegister, this.Registers.B16[instruction.SourceRegister], (a, b) => { return a + b; });
                    break;
                case OpCode.Add_L:
                    this.ExecuteArithmeticOperation(instruction.TargetRegister, instruction.Literal, (a, b) => { return a + b; });
                    break;
                #endregion
                case OpCode.Substract_R:
                    this.Registers.B16[instruction.TargetRegister] -= this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Substract_L:
                    throw new NotImplementedException(); //todo
                case OpCode.Multiply_R:
                    this.Registers.B16[instruction.TargetRegister] *= this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Multiply_L:
                    throw new NotImplementedException(); //todo
                case OpCode.Divide_R:
                    this.Registers.B16[instruction.TargetRegister] /= this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Divide_L:
                    throw new NotImplementedException(); //todo
                case OpCode.And_R:
                    this.Registers.B16[instruction.TargetRegister] &= this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.And_L:
                    throw new NotImplementedException(); //todo
                case OpCode.Or_R:
                    this.Registers.B16[instruction.TargetRegister] |= this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Or_L:
                    throw new NotImplementedException(); //todo
                case OpCode.Not_R:
                    this.Registers.B16[instruction.TargetRegister] = (ushort)~(int)this.Registers.B16[instruction.SourceRegister];
                    break;
            }
        }

        private void ExecuteArithmeticOperation(RegisterCodes targetRegister, ushort otherValue, Func<ushort, ushort, int> predicate)
        {
            ushort targetValue = this.Registers.B16[targetRegister];

            int result = predicate(targetValue, otherValue);
            this.Registers.B16[targetRegister] = (ushort)result;

            this.Registers.SetFlag(AluFlags.Zero, result == 0);
            this.Registers.SetFlag(AluFlags.Carry, result > ushort.MaxValue);
            this.Registers.SetFlag(AluFlags.HalfCarry, result > byte.MaxValue);
            this.Registers.SetFlag(AluFlags.Parity, result % 2 == 1);
        }

        private void DoJump(ushort literalAddress, AluFlags condition, bool acceptValue)
        {
            if(this.Registers.GetFlag(condition) == acceptValue)
            {
                this.Registers.B16[RegisterCodes.PC] = literalAddress;
            }
        }

        private void DoJump(RegisterCodes registerAddress, AluFlags condition, bool acceptValue)
        {
            if(this.Registers.GetFlag(condition) == acceptValue)
            {
                this.Registers.B16[RegisterCodes.PC] = this.Registers.B16[registerAddress];
            }
        }
    }
}
