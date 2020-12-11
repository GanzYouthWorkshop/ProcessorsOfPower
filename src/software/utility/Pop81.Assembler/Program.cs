using Pop.CompilerApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Pop81.Assembler.Pop81AssemblyTokenizer;

namespace Pop81.Assembler
{
    class Program
    {

        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                string assemblyFile = string.Empty;
                bool verbose = false;
                string outputFile = string.Empty;

                for (int i = 0; i < args.Length; i++)
                {
                    if(i == 0)
                    {
                        assemblyFile = File.ReadAllText(args[0]);
                        outputFile = args[0] + ".bin";
                    }
                    else
                    {
                        switch(args[i].ToLower())
                        {
                            case "--verbose":
                                verbose = true;
                                break;
                            case "--output":
                                if(args.Length > i)
                                {
                                    outputFile = args[i + 1];
                                }
                                break;
                        }
                    }
                }

                //for (int i = 0; i < 16; i++)
                //{
                //    for (int j = 0; j < 16; j++)
                //    {
                //        int code = i * 16 + j;
                //        Console.Write($"\u001b[38;5;{code}m {code.ToString().PadLeft(4)}");
                //    }
                //}

                Pop81AssemblyTokenizer tokenizer = new Pop81AssemblyTokenizer()
                {
                    IsVerbose = verbose
                };
                List<Token<TokenTypes>> outputTokens = tokenizer.Run(assemblyFile);
                List<Token<TokenTypes>> escapedOutput = tokenizer.RemoveTokenTypes(outputTokens, TokenTypes.Comment, TokenTypes.Whitespace, TokenTypes.NewLine);

                if(tokenizer.IsVerbose)
                {
                    tokenizer.ListTokens(escapedOutput);
                }

                Assembler asm = new Assembler()
                {
                    IsVerbose = verbose
                };
                asm.Run(escapedOutput);

                if (true)//asm.Erros == 0)
                {
                    File.WriteAllBytes(outputFile, asm.Binary.ToArray());
                }
            }
        }        
    }
}
