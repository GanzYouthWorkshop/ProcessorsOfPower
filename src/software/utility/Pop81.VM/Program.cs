using Pop81.VM;
using Pop81.VM.Implementation;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

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

            m_Machine = new VirtualMachine();

            if (args.Length > 0)
            {
                byte[] ROM = File.ReadAllBytes(args[0]);
                ROM.CopyTo(m_Machine.MainMemory, 0);
            }

            m_Machine.Start();

            m_Display = new Display()
            {
                Machine = m_Machine
            };
            m_Display.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
