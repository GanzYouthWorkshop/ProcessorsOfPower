using Pop.CompilerApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pop81.Assembler
{
    public class Pop81AssemblyTokenizer
    {
        public enum TokenTypes
        {
            Instruction,
            Comment,
            Whitespace,
            NewLine,

            Opcode,
            Directive,

            IndirectAddressing,

            Register8,
            Register16,
            Label,
            LiteralDecimal,
            LiteralBinary1,
            LiteralBinary2,
            LiteralHex1,
            LiteralHex2,
            LiteralKay,
            LiteralLabel,
        }

        public Tokenizer<TokenTypes> Tokenizer
        {
            get
            {
                if(this.m_Tokenizer == null)
                {
                    string opcodes = String.Join('|', Enum.GetValues(typeof(Mnemonics)).Cast<Mnemonics>().Select(m => m.ToString()));
                    string registers8 = String.Join('|', Enum.GetValues(typeof(RegisterCodes)).Cast<RegisterCodes>().Where(r => ((byte)r & Register.BITS16) == 0).Select(r => r.ToString()));
                    string registers16 = String.Join('|', Enum.GetValues(typeof(RegisterCodes)).Cast<RegisterCodes>().Where(r => ((byte)r & Register.BITS16) > 0).Select(r => r.ToString()));

                    this.m_Tokenizer = new Tokenizer<TokenTypes>()
                    {
                        TokenDefinitions = new List<TokenDefinition<TokenTypes>>()
                        {
                            new TokenDefinition<TokenTypes>(TokenTypes.Instruction, new Regex(@"^(?!;).+$", RegexOptions.Multiline), 99),
                            new TokenDefinition<TokenTypes>(TokenTypes.NewLine, new Regex(@"[\n|\r\n]+")),

                            new TokenDefinition<TokenTypes>(TokenTypes.Comment, new Regex(@";.*$")),
                            new TokenDefinition<TokenTypes>(TokenTypes.Whitespace, new Regex(@"\s+")),

                            new TokenDefinition<TokenTypes>(TokenTypes.Opcode, new Regex(@$"({opcodes})", RegexOptions.IgnoreCase)),
                            new TokenDefinition<TokenTypes>(TokenTypes.Directive, new Regex(@"@[a-z0-9]+")),

                            new TokenDefinition<TokenTypes>(TokenTypes.Label, new Regex(@"[a-zA-Z]+\:")),

                            new TokenDefinition<TokenTypes>(TokenTypes.IndirectAddressing, new Regex(@"\[[a-z0-9\$]+\]"), 1),

                            new TokenDefinition<TokenTypes>(TokenTypes.Register8, new Regex(@$"({registers8})", RegexOptions.IgnoreCase)),
                            new TokenDefinition<TokenTypes>(TokenTypes.Register16, new Regex(@$"({registers16})", RegexOptions.IgnoreCase)),

                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralLabel, new Regex(@":[a-zA-Z][\w]+")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralBinary1, new Regex("0b[01]+")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralBinary2, new Regex("[01]+b")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralHex1, new Regex("0x[0-9a-fA-F]+")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralHex2, new Regex("[0-9a-fA-F]+h")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralKay, new Regex(@"\$[0-9]+[kM][0-9]*")),
                            new TokenDefinition<TokenTypes>(TokenTypes.LiteralDecimal, new Regex("[0-9]+")),

                        }
                    };
                }

                return this.m_Tokenizer;
            }
        } 
        private Tokenizer<TokenTypes> m_Tokenizer;

        public bool IsVerbose { get; set; }

        public List<Token<TokenTypes>> Run(string source)
        {
            return this.Tokenizer.Tokenize(source);
        }

        public List<Token<TokenTypes>> RemoveTokenTypes(List<Token<TokenTypes>> tokens, params TokenTypes[] types)
        {
            List<Token<TokenTypes>> result = new List<Token<TokenTypes>>();

            foreach (Token<TokenTypes> token in tokens)
            {
                if(!types.Contains(token.TokenType))
                {
                    if(token.Tokens != null)
                    {
                        token.Tokens = this.RemoveTokenTypes(token.Tokens, types);
                    }

                    result.Add(token);
                }
            }

            return result;
        }

        public void ListTokens(List<Token<TokenTypes>> tokens, int indent = 0)
        {
            if (indent == 0)
            {
                Console.WriteLine("ROOT");
            }

            for(int i = 0; i < tokens.Count; i++)
            {
                char c = i == tokens.Count - 1 ? '└' : '├';
                Console.WriteLine($"{new String('│', indent)}{c}\u001b[38;5;{Utils.INSTR_COLOR}m{tokens[i].TokenType} \u001b[38;5;{Utils.FROM_REG_COLOR}m{tokens[i].Value}\u001b[0m");
                if(tokens[i].Tokens != null)
                {
                    this.ListTokens(tokens[i].Tokens, indent + 1);
                }
            }
        }
    }
}
