using Pop.CompilerApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Pop81.Assembler.Pop81AssemblyTokenizer;

namespace Pop81.Assembler
{
    public class AssemblerOld
    {
        private Dictionary<string, int> m_Labels = new Dictionary<string, int>();

        public int Erros { get; private set; } = 0;
        public List<byte> Binary { get; private set; } = new List<byte>();
        public bool IsVerbose { get; set; }

        public void Run(List<Token<TokenTypes>> tokens)
        {

        }

        public void Run(string[] sourceCode)
        {
            Regex labelReference = new Regex(@":([\w\d]+)", RegexOptions.Compiled);

            #region First run - line cleaning, labels, control expressions
            List<string> cleanedSource = new List<string>();
            for (int i = 0; i < sourceCode.Length; i++)
            {
                #region line cleaning
                string cleanLine = sourceCode[i].Replace("\t", "");
                if (cleanLine.IndexOf(';') > -1)
                {
                    cleanLine = cleanLine.Substring(0, cleanLine.IndexOf(';'));
                }
                sourceCode[i] = cleanLine;

                if(!string.IsNullOrEmpty(cleanLine))
                {
                    cleanedSource.Add(cleanLine);
                }
                #endregion
            }
            sourceCode = cleanedSource.ToArray();
            #endregion

            int addresses = 0;
            for (int i = 0; i < sourceCode.Length; i++)
            {
                #region label definitions
                if (sourceCode[i].EndsWith(':'))
                {
                    m_Labels.Add(sourceCode[i].Substring(0, sourceCode[i].Length - 1).ToLower(), addresses);
                    sourceCode[i] = string.Empty;
                }
                addresses += 4;
                #endregion
            }

            #region Second run - label references
            for (int i = 0; i < sourceCode.Length; i++)
            {
                sourceCode[i] = labelReference.Replace(sourceCode[i], (m) =>
                {
                    return m_Labels[m.Groups[1].Value].ToString();
                });
            }
            #endregion

            #region Third run - asm to binary; only mnemonics left at this point
            foreach (string line in sourceCode)
            {
                this.ParseLine(line);
            }
            #endregion
        }

        private byte[] ParseLine(string assemblyLine)
        {
            byte[] result = null;

            if (!string.IsNullOrEmpty(assemblyLine))
            {
                string[] frags = assemblyLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                MachineInstruction instruction = new MachineInstruction();

                //todo check mnemonic
                //todo check if target and/or source is literal or is a register, or an indirect value
                //todo compile indirect value instructions into direct-valued instructions (load first)
                //todo use mnemonic->opcode translation for various register lengths, literals
                //todo put together 4 bytes for machine language instruction(s)

                Address target = null;
                Address source = null;

                if (frags.Length > 1)
                {
                    target = Address.FromString(frags[1]);
                }
                if (frags.Length > 2)
                {
                    source = Address.FromString(frags[2]);
                }

                Address oneLiteal = new Address(Address.Types.Literal, 16, 1);

                switch (frags[0].ToUpper())
                {
                    case nameof(Mnemonics.NOPE):
                        instruction.Opcode = OpCode.Nop_X;
                        break;
                    case nameof(Mnemonics.HALT):
                        instruction.Opcode = OpCode.Halt_X;
                        break;
                    //case nameof(Mnemonics.MOVE):
                    //    instruction = this.TwoOperands(OpCode.Move_R, OpCode.Move_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.JUMP):
                    //    Address programCounter = new Address(Address.Types.Register, 8, (byte)RegisterCodes.PC);
                    //    instruction = this.TwoOperands(OpCode.Move_R, OpCode.Move_L, programCounter, target);
                    //    break;

                    //#region Arithmethic & logical
                    //case nameof(Mnemonics.ADD):
                    //    instruction = this.TwoOperands(OpCode.Add_R, OpCode.Add_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.SUB):
                    //    instruction = this.TwoOperands(OpCode.Substract_R, OpCode.Substract_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.INCR):
                    //    instruction = this.TwoOperands(OpCode.Add_R, OpCode.Add_L, target, oneLiteal);
                    //    break;
                    //case nameof(Mnemonics.DECR):
                    //    instruction = this.TwoOperands(OpCode.Substract_R, OpCode.Substract_L, target, oneLiteal);
                    //    break;
                    //case nameof(Mnemonics.DIV):
                    //    instruction = this.TwoOperands(OpCode.Divide_R, OpCode.Divide_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.MUL):
                    //    instruction = this.TwoOperands(OpCode.Multiply_R, OpCode.Multiply_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.AND):
                    //    instruction = this.TwoOperands(OpCode.And_R, OpCode.And_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.OR):
                    //    instruction = this.TwoOperands(OpCode.Or_R, OpCode.Or_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.NOT):
                    //    instruction = this.OneOperand(OpCode.Not_R, target);
                    //    break;
                    //case nameof(Mnemonics.LSFT):
                    //    instruction = this.TwoOperands(OpCode.LShift_R, OpCode.LShift_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.RSFT):
                    //    instruction = this.TwoOperands(OpCode.RShift_R, OpCode.RShift_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.LROT):
                    //    instruction = this.TwoOperands(OpCode.LRot_R, OpCode.LRot_L, target, source);
                    //    break;
                    //case nameof(Mnemonics.RROT):
                    //    instruction = this.TwoOperands(OpCode.RRot_R, OpCode.RRot_L, target, source);
                    //    break;
                    //#endregion

                    default:
                        Console.WriteLine($"\u001b[38;5;9mUnknown mnemonic found: {frags[0].ToUpper()}!\u001b[0m");
                        break;
                }
                result = BitConverter.GetBytes(instruction.Data);

                if (this.IsVerbose)
                {
                    Console.WriteLine($"{FormatToDebug(frags, this.Binary.Count)} | {instruction.FormatToDebug()}");
                }
            }

            if (result != null)
            {
                Binary.AddRange(result);
            }
            return result;
        }

        //private MachineInstruction[] OneOperand(OpCode code, Address target)
        //{
        //    MachineInstruction result = new MachineInstruction();

        //    result.Opcode = code;
        //    result.Target = target.B8;
        //    result.Literal = 0;

        //    return result;
        //}

        //private MachineInstruction[] TwoOperands(OpCode withRegister, OpCode withLiteral, Address target, Address source)
        //{
        //    MachineInstruction result = new MachineInstruction();

        //    if (source.Type == Address.Types.Register)
        //    {
        //        result.Opcode = withRegister;
        //        result.Target = target.B8;
        //        result.Source = target.B8;
        //    }
        //    else
        //    {
        //        result.Opcode = withLiteral;
        //        result.Target = target.B8;
        //        result.Literal = source.B16;
        //    }

        //    return result;
        //}

        //private MachineInstruction[] ReadMemory()
        //{
        //    //set MA
        //    //load
        //    //set target regiszter

        //    MachineInstruction result = new MachineInstruction
        //    {
        //        result.Opcode = OpCode.Move_R,
        //        result.Target = RegisterCodes.MA,
        //    };
        //    MachineInstruction result = new MachineInstruction
        //    {
        //        result.Opcode = OpCode.Move_R,
        //        result.Target = RegisterCodes.MA,
        //    };

        //}

        public static ushort LiteralToShort(string literal)
        {
            ushort result = 0;

            if (literal.StartsWith("$"))
            {
                string[] cleans = literal.Substring(1).Split("k");
                if (cleans.Length > 1 && cleans[1] == "")
                {
                    cleans[1] = "0";
                }
                result = (ushort)(Convert.ToUInt16(cleans[0], 10) * 1024 + Convert.ToUInt16(cleans[1], 10));
            }
            else if (literal.StartsWith("0x") || literal.EndsWith("h"))
            {
                string clean = literal.Replace("0x", "").Replace("h", "");
                result = Convert.ToUInt16(clean, 16);
            }
            else if (literal.StartsWith("0b") || literal.EndsWith("b"))
            {
                string clean = literal.Replace("0b", "").Replace("b", "");
                result = Convert.ToUInt16(clean, 2);

            }
            else
            {
                result = Convert.ToUInt16(literal, 10);
            }

            return result;
        }

        public string FormatToDebug(string[] frags, int address)
        {
            int[] colors = new int[]
            {
                Utils.INSTR_COLOR,
                Utils.TO_REG_COLOR,
                Utils.FROM_REG_COLOR
            };

            string displayAddress = address.ToString("X4");
            int color = Utils.DONTCARE_COLOR;
            if(this.m_Labels.ContainsValue(address))
            {
                color += 16;
            }
            string result = $"\u001b[38;5;{color}m{displayAddress}\u001b[0m | ";

            int visibleLength = 0;
            for(int i = 0; i < frags.Length; i++)
            {
                string display = i == 0 ? frags[i].PadRight(4) : frags[i];
                visibleLength += display.Length + 1;
                result += $"\u001b[38;5;{colors[i]}m{display}\u001b[0m ";
            }

            result += new String(' ', 30 - visibleLength);
            return result;
        }
    }
}
