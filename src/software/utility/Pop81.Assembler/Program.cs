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
                string assemblyFile = File.ReadAllText(args[0]);
                assemblyFile = assemblyFile.Replace("\r", "");
                string[] lines = assemblyFile.Split("\n");

                //for (int i = 0; i < 16; i++)
                //{
                //    for (int j = 0; j < 16; j++)
                //    {
                //        int code = i * 16 + j;
                //        Console.Write($"\u001b[38;5;{code}m {code.ToString().PadLeft(4)}");
                //    }
                //}


                //AssemblerOld asm = new AssemblerOld()
                //{
                //    IsVerbose = true,
                //};
                //asm.Run(lines);

                Pop81AssemblyTokenizer tokenizer = new Pop81AssemblyTokenizer()
                {
                    IsVerbose = true
                };
                List<Token<TokenTypes>> output = tokenizer.Run(assemblyFile);
                List<Token<TokenTypes>> escapedOutput = tokenizer.RemoveTokenTypes(output, TokenTypes.Comment, TokenTypes.Whitespace, TokenTypes.NewLine);

                if(tokenizer.IsVerbose)
                {
                    tokenizer.ListTokens(escapedOutput);
                }

                Assembler asm = new Assembler()
                {
                    IsVerbose = true
                };
                asm.Run(escapedOutput);



                if (true)//asm.Erros == 0)
                {
                    File.WriteAllBytes(args[0] + ".bin", asm.Binary.ToArray());
                }
            }
        }        
    }
}
