using Pop.CompilerApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Pop81.Assembler.Pop81AssemblyTokenizer;

namespace Pop81.Assembler
{
    public class Assembler
    {
        public List<byte> Binary { get; private set; } = new List<byte>();
        public bool IsVerbose { get; set; }

        private Dictionary<string, int> m_Labels = new Dictionary<string, int>();

        public void Run(List<Token<TokenTypes>> tokens)
        {
            #region 1. futás - direktívák és címkék
            int addresses = 0;

            List<Token<TokenTypes>> escapedTokens = new List<Token<TokenTypes>>();

            foreach (Token<TokenTypes> token in tokens)
            {
                if (token.TokenType == TokenTypes.Instruction)
                {
                    if (token.Tokens.Count > 0)
                    {
                        switch (token.Tokens[0].TokenType)
                        {
                            case TokenTypes.Directive:
                                break;

                            case TokenTypes.Label:
                                string label = token.Tokens.First().Value;
                                label = label.Substring(0, label.Length - 1).ToLower();

                                if (this.m_Labels.ContainsKey(label))
                                {
                                    //todo error
                                }
                                else
                                {
                                    this.m_Labels.Add(label, addresses);
                                }
                                break;

                            case TokenTypes.Opcode:
                                List<Token<TokenTypes>> newInstructions = new List<Token<TokenTypes>>();

                                if (token.Tokens.Count > 1 && token.Tokens[1].TokenType == TokenTypes.IndirectAddressing)                          
                                {
                                    newInstructions = new List<Token<TokenTypes>>()
                                    {
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "MOVE" },
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Register8, Value = "md" },
                                                token.Tokens[2],
                                            }
                                        },
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "MOVE" },
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Register16, Value = "ma" },
                                                token.Tokens[1].Tokens[0],
                                            }
                                        },
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "SAVE" },
                                            }
                                        },
                                    };
                                }
                                else if((token.Tokens.Count > 2 && token.Tokens[2].TokenType == TokenTypes.IndirectAddressing))
                                {
                                    newInstructions = new List<Token<TokenTypes>>()
                                    {
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "MOVE" },
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Register16, Value = "ma" },
                                                token.Tokens[2].Tokens[0]
                                            }
                                        },
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "LOAD" },
                                            }
                                        },
                                        new Token<TokenTypes>()
                                        {
                                            TokenType = TokenTypes.Instruction,
                                            Tokens = new List<Token<TokenTypes>>()
                                            {
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Opcode, Value = "MOVE" },
                                                token.Tokens[1],
                                                new Token<TokenTypes>(){ TokenType = TokenTypes.Register8, Value = "md" },
                                            }
                                        },
                                    };
                                }
                                else
                                {
                                    newInstructions.Add(token);
                                }

                                escapedTokens.AddRange(newInstructions);
                                addresses = escapedTokens.Count * 4;
                                break;
                        }
                    }
                }
            }
            #endregion

            #region 2. futás - utasítások
            addresses = 0;

            foreach (Token<TokenTypes> token in escapedTokens)
            {
                MachineInstruction instruction = new MachineInstruction();

                if (token.TokenType == TokenTypes.Instruction &&
                    token.Tokens.Count > 0 &&
                    token.Tokens[0].TokenType == TokenTypes.Opcode)
                {
                    switch(token.Tokens[0].Value)
                    {
                        case nameof(Mnemonics.NOPE):
                            instruction = new MachineInstruction() { Opcode = OpCode.Nop_X };
                            break;
                        case nameof(Mnemonics.HALT):
                            instruction = new MachineInstruction() { Opcode = OpCode.Halt_X };
                            break;

                        case nameof(Mnemonics.SAVE):
                            instruction = new MachineInstruction() { Opcode = OpCode.Store };
                            break;
                        case nameof(Mnemonics.LOAD):
                            instruction = new MachineInstruction() { Opcode = OpCode.Load };
                            break;

                        case nameof(Mnemonics.MOVE):
                            instruction = this.GenerateInstruction(2, OpCode.Move_R, OpCode.Move_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.JUMP):
                            token.Tokens.Insert(1, new Token<TokenTypes>(){ TokenType = TokenTypes.Register16, Value = "pc", });
                            instruction = this.GenerateInstruction(2, OpCode.Move_R, OpCode.Move_L, token.Tokens);
                            break;

                        case nameof(Mnemonics.ADD):
                            instruction = this.GenerateInstruction(2, OpCode.Add_R, OpCode.Add_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.SUB):
                            instruction = this.GenerateInstruction(2, OpCode.Substract_R, OpCode.Substract_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.INCR):
                            instruction = this.GenerateInstruction(2, OpCode.Add_R, OpCode.Add_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.DECR):
                            instruction = this.GenerateInstruction(2, OpCode.Substract_R, OpCode.Substract_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.DIV):
                            instruction = this.GenerateInstruction(2, OpCode.Divide_R, OpCode.Divide_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.MUL):
                            instruction = this.GenerateInstruction(2, OpCode.Multiply_R, OpCode.Multiply_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.AND):
                            instruction = this.GenerateInstruction(2, OpCode.And_R, OpCode.And_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.OR):
                            instruction = this.GenerateInstruction(2, OpCode.Or_R, OpCode.Or_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.NOT):
                            instruction = this.GenerateInstruction(1, OpCode.Not_R, OpCode.Not_R, token.Tokens);
                            break;
                        case nameof(Mnemonics.LSFT):
                            instruction = this.GenerateInstruction(2, OpCode.LShift_R, OpCode.LShift_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.RSFT):
                            instruction = this.GenerateInstruction(2, OpCode.RShift_R, OpCode.RShift_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.LROT):
                            instruction = this.GenerateInstruction(2, OpCode.LRot_R, OpCode.LRot_L, token.Tokens);
                            break;
                        case nameof(Mnemonics.RROT):
                            instruction = this.GenerateInstruction(2, OpCode.RRot_R, OpCode.RRot_L, token.Tokens);
                            break;

                        default:
                            Console.WriteLine($"\u001b[38;5;9mUnknown mnemonic found: {token.Tokens[0].Value.ToUpper()}!\u001b[0m");
                            break;
                    }
                }

                if (this.IsVerbose)
                {
                    Console.WriteLine($"{FormatToDebug(token, this.Binary.Count)} | {instruction.FormatToDebug()}");
                }

                this.Binary.Add(instruction.Byte1);
                this.Binary.Add(instruction.Byte2);
                this.Binary.Add(instruction.Byte3);
                this.Binary.Add(instruction.Byte4);

                addresses += 4;
            }
            #endregion
        }

        public MachineInstruction GenerateInstruction(int operands, OpCode registerBasedCode, OpCode literalBasedCode, List<Token<TokenTypes>> tokens)
        {
            MachineInstruction result = new MachineInstruction();

            if (tokens.Count < operands + 1)
            {
                //todo error
            }
            else
            {
                Token<TokenTypes> sourceOperand = tokens.Last();

                List<TokenTypes> registerTypeTokens = new List<TokenTypes>()
                {
                    TokenTypes.Register8,
                    TokenTypes.Register16,
                };

                List<TokenTypes> literalTypTokens = new List<TokenTypes>()
                {
                    TokenTypes.LiteralBinary1,
                    TokenTypes.LiteralBinary2,
                    TokenTypes.LiteralDecimal,
                    TokenTypes.LiteralHex1,
                    TokenTypes.LiteralHex2,
                    TokenTypes.LiteralKay,
                    TokenTypes.LiteralLabel,
                };

                if (registerTypeTokens.Contains(sourceOperand.TokenType))
                {
                    result.Opcode = registerBasedCode;
                    result.SourceRegister = (byte)this.GetRegister(sourceOperand);

                    if(operands == 2)
                    {
                        result.TargetRegister = (byte)this.GetRegister(tokens[1]); 
                    }
                }
                else if(literalTypTokens.Contains(sourceOperand.TokenType))
                {
                    result.Opcode = literalBasedCode;
                    result.Literal = this.LiteralToShort(sourceOperand);

                    if (operands == 2)
                    {
                        result.TargetRegister = (byte)this.GetRegister(tokens[1]);
                    }
                }
                else
                {
                    //todo error
                }
            }

            return result;
        }

        public ushort LiteralToShort(Token<TokenTypes> literal)
        {
            ushort result = 0;

            if (literal.TokenType == TokenTypes.LiteralKay)
            {
                string[] cleans = literal.Value.Substring(1).Split("k");
                if (cleans.Length > 1 && cleans[1] == "")
                {
                    cleans[1] = "0";
                }
                result = (ushort)(Convert.ToUInt16(cleans[0], 10) * 1024 + Convert.ToUInt16(cleans[1], 10));
            }
            else if (literal.TokenType == TokenTypes.LiteralHex1 || literal.TokenType == TokenTypes.LiteralHex2)
            {
                string clean = literal.Value.Replace("0x", "").Replace("h", "");
                result = Convert.ToUInt16(clean, 16);
            }
            else if (literal.TokenType == TokenTypes.LiteralBinary1 || literal.TokenType == TokenTypes.LiteralBinary2)
            {
                string clean = literal.Value.Replace("0b", "").Replace("b", "");
                result = Convert.ToUInt16(clean, 2);

            }
            else if(literal.TokenType == TokenTypes.LiteralDecimal)
            {
                result = Convert.ToUInt16(literal.Value, 10);
            }

            return result;
        }

        private RegisterCodes GetRegister(Token<TokenTypes> token)
        {
            return Enum.GetValues(typeof(RegisterCodes)).Cast<RegisterCodes>().First(c => c.ToString().ToLower() == token.Value);
        }

        public string FormatToDebug(Token<TokenTypes> token, int address)
        {
            int[] colors = new int[]
            {
                Utils.INSTR_COLOR,
                Utils.TO_REG_COLOR,
                Utils.FROM_REG_COLOR,
                Utils.DONTCARE_COLOR,
            };

            string displayAddress = address.ToString("X4");
            int color = Utils.DONTCARE_COLOR;
            if (this.m_Labels.ContainsValue(address))
            {
                color += 16;
            }
            string result = $"\u001b[38;5;{color}m{displayAddress}\u001b[0m | ";

            int visibleLength = 0;
            foreach(Token<TokenTypes> frag in token.Tokens)
            {
                int  c = 0;
                bool pad = false;
                switch(frag.TokenType)
                {
                    case TokenTypes.Opcode:
                        c = 0;
                        pad = true;
                        break;
                    case TokenTypes.Register8:
                    case TokenTypes.Register16:
                        c = 1;
                        break;
                    case TokenTypes.LiteralDecimal:
                    case TokenTypes.LiteralBinary1:
                    case TokenTypes.LiteralBinary2:
                    case TokenTypes.LiteralHex1:
                    case TokenTypes.LiteralHex2:
                    case TokenTypes.LiteralKay:
                        c = 2;
                        break;
                    case TokenTypes.IndirectAddressing:
                        c = 3;
                        break;
                }

                string display = pad ? frag.Value.PadRight(4) : frag.Value;
                visibleLength += display.Length + 1;
                result += $"\u001b[38;5;{colors[c]}m{display}\u001b[0m ";
            }

            result += new String(' ', 30 - visibleLength);
            return result;
        }

    }
}
