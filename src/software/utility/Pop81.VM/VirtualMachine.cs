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

        public RunMode CurrentMode { get; set; }
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
                case OpCode.Nop_X:
                    return;
                case OpCode.Halt_X:
                    this.CanRun = false; break;

                case OpCode.Jump_R:
                case OpCode.Jump_L:
                case OpCode.JumpIfZero_R:
                case OpCode.JumpIfZero_L:
                case OpCode.JumpIfCarry:
                case OpCode.JumpIfNotCarry:
                    throw new NotImplementedException(); //todo

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

                case OpCode.Move_R:
                    this.Registers.B16[instruction.TargetRegister] = this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Move_L:
                    this.Registers.B16[instruction.TargetRegister] = instruction.Literal;
                    break;

                case OpCode.Add_R:
                    this.Registers.B16[instruction.TargetRegister] += this.Registers.B16[instruction.SourceRegister];
                    break;
                case OpCode.Add_L:
                    throw new NotImplementedException(); //todo
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
    }
}
