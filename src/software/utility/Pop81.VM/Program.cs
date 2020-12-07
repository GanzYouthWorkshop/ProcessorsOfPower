using Pop81.VM;
using Pop81.VM.Implementation;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Pop8
{
    public class Program
    {
        private static VirtualMachine m_Machine;
        private static Display m_Display;

        static void Main(string[] args)
        {
            //Read file
            //Execute each 4 bytes as an instruction
            //Display the "graphics memory"
            //run until EOF of HALT or indefinitely

            //if(args.Length > 0)
            //{
            //    byte[] ROM = File.ReadAllBytes(args[0]);
            //    ROM.CopyTo(m_Memory, 0);
            //}
            //int pc = 0; //

            m_Machine = new VirtualMachine();

            m_Display = new Display()
            {
                Machine = m_Machine
            };
            m_Display.Start();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
            }
        }
    }
}
