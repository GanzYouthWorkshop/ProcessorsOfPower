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
                    this.Registers.B16[RegisterCodes.PC] = pp;

                    switch (this.CurrentMode)
                    {
                        case RunMode.RealTime: Thread.Sleep(10); break;
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
                    this.DoJump(instruction.TargetRegister, AluFlags.Z, true);
                    break;
                case OpCode.JumpIfZero_L:
                    this.DoJump(instructin.Literal, AluFlags.Z, true);
                    break;
                #endregion
                #region JINZ
                #endregion
                #region JIFG
                #endregion
                #region JING
                #endregion
                #region JIFC
                #endregion
                #region JINC
                #endregion
                #region JIFP
                #endregion
                #region JINP
                #endregion
                #region JIFS
                #endregion
                #region JINS
                #endregion
                #region JIFH
                #endregion
                #region JINH
                #endregion
                case OpCode.JumpIfCarry:
                case OpCode.JumpIfNotCarry:
                    throw new NotImplementedException(); //todo

                case OpCode.Compare_R:
                    ushort a = this.Registers.B16[instruction.TargetRegister];
                    ushort b = this.Registers.B16[instruction.SourceRegister];

                    this.Registers.SetFlag(AluFlags.Zero, a == b);
                    break;

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

                #region MOVE
                case OpCode.Move_R:
                    this.Registers.B16[instruction.TargetRegister] = this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Move_L:
                    this.Registers.B16[instruction.TargetRegister] = instruction.Literal;
                    break;
                #endregion
                
                case OpCode.Add_R:
                    this.ExecuteArithmeticOperation(instruction.TargetRegister, this.Registers.B16[instruction.SourceRegister], (a, b) => { return a + b; });
                    break;
                case OpCode.Add_L:
                    this.ExecuteArithmeticOperation(instruction.TargetRegister, instruction.Literal, (a, b) => { return a + b; });
                    break;
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

            int result  = predicate(targetValue, otherValue);
            this.Registers.B16[targetRegister] = (ushort)result;

            this.Registers.SetFlag(AluFlags.Zero, result == 0);
            this.Registers.SetFlag(AluFlags.Carry, result > ushort.MaxValue);
            this.Registers.SetFlag(AluFlags.HalfCarry, result > byte.MaxValue);
            this.Registers.SetFlag(AluFlags.Parity, result % 2 == 1);
        private void DoJump(ushort literalAddress, AluFlags condition, bool accepValue)
        {
            if(this.Registers.GetFlag(condition) == acceptValue)
            {
                this.Registers.B16[RegisterCodes.PC] = literalAddress;
            }
        }

        private void DoJump(RegisterCodes registerAddress, AluFlags condition, bool accepValue)
        {
            if(this.Registers.GetFlag(condition) == acceptValue)
            {
                this.Registers.B16[RegisterCodes.PC] = this.Registers.B16[registerAddress];
            }
        }
    }
}
