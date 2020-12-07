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

        public byte[] MainMemory { get; } = new byte[32 * 1034];
        public Dictionary<RegisterCodes, Register> Registers { get; } = new Dictionary<RegisterCodes, Register>();

        public RunMode CurrentMode { get; set; }
        public AutoResetEvent Ticker { get; } = new AutoResetEvent(false);

        public bool CanRun { get; set; } = true;

        public void Start()
        {
            this.Registers[RegisterCodes.RA] = new Register();
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

                    int pp = this.Registers[RegisterCodes.PP].B16;
                    byte[] m = this.MainMemory;

                    MachineInstruction instruction = new MachineInstruction(new byte[] { m[pp++], m[pp++], m[pp++], m[pp++] });
                    this.ExecuteInstruction(instruction);

                    if (this.CurrentMode == RunMode.RealTime)
                    {
                        Thread.Sleep(10);
                    }
                    else if (this.CurrentMode == RunMode.Slow)
                    {
                        Thread.Sleep(200);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void ExecuteInstruction(MachineInstruction instruction)
        {
            switch(instruction.Opcode)
            {
                case OpCode.Nop:
                    return;
                case OpCode.Halt:
                    this.CanRun = false; break;

                case OpCode.JumpRegister:
                case OpCode.JumpLiteral:
                case OpCode.JumpIfZero:
                case OpCode.JumpIfNotZero:
                case OpCode.JumpIfCarry:
                case OpCode.JumpIfNotCarry:
                    throw new NotImplementedException(); //todo

                case OpCode.Pop:
                    this.Registers[instruction.Target].B8 = this.MainMemory[this.Registers[RegisterCodes.DSI].B16];
                    this.Registers[RegisterCodes.DSI].B16--;
                    break;
                case OpCode.Push:
                    this.Registers[RegisterCodes.DSI].B16++;
                    this.MainMemory[this.Registers[RegisterCodes.DSI].B16] = this.Registers[instruction.Target].B8;
                    break;


                case OpCode.Call:
                    this.Registers[instruction.Target].B8 = this.MainMemory[this.Registers[RegisterCodes.DSI].B16];
                    this.Registers[RegisterCodes.DSI].B16--;
                    break;
                case OpCode.Return:
                    this.Registers[RegisterCodes.DSI].B16++;
                    this.MainMemory[this.Registers[RegisterCodes.DSI].B16] = this.Registers[instruction.Target].B8;
                    break;

                case OpCode.Load:
                    this.Registers[RegisterCodes.MD].B8 = this.MainMemory[this.Registers[RegisterCodes.MA].B16];
                    break;
                case OpCode.Store:
                    this.MainMemory[this.Registers[RegisterCodes.MA].B16] = this.Registers[RegisterCodes.MD].B8;
                    break;

                case OpCode.MoveRegister:
                    this.Registers[instruction.Target].B8 = this.Registers[instruction.Source].B8;
                    break;
                case OpCode.MoveLiteral:
                    //this.Registers[instruction.Target].B8 = instruction.Literal; //todo
                    throw new NotImplementedException(); //todo
                    break;

                case OpCode.AddRegister:
                    this.Registers[instruction.Target].B8 += this.Registers[instruction.Source].B8;
                    break;
                case OpCode.AddLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.SubstractRegister:
                    this.Registers[instruction.Target].B8 -= this.Registers[instruction.Source].B8;
                    break;
                case OpCode.SubstractLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.MultiplyRegister:
                    this.Registers[instruction.Target].B8 *= this.Registers[instruction.Source].B8;
                    break;
                case OpCode.MultiplyLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.DivideRegister:
                    this.Registers[instruction.Target].B8 /= this.Registers[instruction.Source].B8;
                    break;
                case OpCode.DivideLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.AndRegister:
                    this.Registers[instruction.Target].B8 &= this.Registers[instruction.Source].B8;
                    break;
                case OpCode.AndLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.OrRegister:
                    this.Registers[instruction.Target].B8 |= this.Registers[instruction.Source].B8;
                    break;
                case OpCode.OrLiteral:
                    throw new NotImplementedException(); //todo
                case OpCode.NotRegister:
                    this.Registers[instruction.Target].B8 = (byte)~(int)this.Registers[instruction.Source].B8;
                    break;
            }
        }
    }
}
