using Pop.CompilerApi;
using System;
using System.Collections.Generic;
using System.Text;
using static Pop81.Assembler.Pop81AssemblyTokenizer;

namespace Pop81.Assembler
{
    public class Assembler
    {
        public List<byte> Binary { get; private set; } = new List<byte>();
        public bool IsVerbose { get; set; }

        public void Run(List<Token<TokenTypes>> tokens)
        {
            foreach(Token<TokenTypes> token in tokens)
            {
                if(token.TokenType == TokenTypes.Instruction)
                {
                    if(token.Tokens.Count > 0)
                    {
                        switch(token.Tokens[0].TokenType)
                        {
                            case TokenTypes.Directive:
                                break;
                            case TokenTypes.Label:
                                break;
                            case TokenTypes.Opcode:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
