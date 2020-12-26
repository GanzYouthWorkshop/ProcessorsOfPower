using Pop.ConsoleUtilities;
using Pop81.VM.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Pop81.VM.Implementation.VirtualMachine;

namespace Pop81.VM
{
    public class Display
    {
        public enum Modes
        {
            Vram,
            MemoryDump,
            Instructions,
            Test,
        }

        private ConsoleBuffer m_Buffer = new ConsoleBuffer();

        public const int VRAM_START = 5000;

        public const int WIDTH = 200;
        public const int HEIGHT = 50;

        public VirtualMachine Machine { get; set; }
        public Modes CurrentMode { get; set; }

        int lastW = 0;
        int lastH = 0;

        public void Draw()
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;

            if(lastW != w || lastH != h)
            {
                lastW = w;
                lastH = h;

                this.m_Buffer.Resize(w, h);
            }

            this.m_Buffer.Clear();

            //Draw header
            this.m_Buffer.Seek(0, 0).Color(0, 15).Draw("Pop81 VM");

            //Drawing frame
            this.m_Buffer.Seek(0, 1).Frame(w, h - 2, true);

            //Drawing footer
            Dictionary<Modes, Tuple<string, string>> FooterMenu = new Dictionary<Modes, Tuple<string, string>>()
            {
                { Modes.Vram, new Tuple<string, string>("F1", "Graphics display") },
                { Modes.Instructions, new Tuple<string, string>("F2", "Instructions") },
                { Modes.MemoryDump, new Tuple<string, string>("F3", "Memory dump") },
                { Modes.Test, new Tuple<string, string>("F4", "Test display") },
            };

            this.m_Buffer.Seek(0, h - 1).Draw("| ");
            foreach (KeyValuePair< Modes, Tuple<string, string>> kvp in FooterMenu)
            {
                int background = this.CurrentMode == kvp.Key ? 11 : 0;
                int foreground = this.CurrentMode == kvp.Key ? 0 : 15;

                this.m_Buffer.Color(background, foreground).Draw(kvp.Value.Item1).Color(0, 255).Draw($" - {kvp.Value.Item2} | ");
            }

            //Draw mian container
            switch(this.CurrentMode)
            {
                case Modes.Vram:
                    #region VRAM - display a specific region of main memory either as ASCII text of colored rectangles
                    this.m_Buffer.Seek(1, 2).Foreground(255).Frame(82, 27, false);
                    //copy memory directly to buffer
                    #endregion
                    break;
                case Modes.MemoryDump:
                    #region Memory dump display a specific region of main memory as a memory dump (address, hey and bynary)
                    this.m_Buffer.Seek(1, 2).Foreground(87).Frame(82, 27, false);

                    this.m_Buffer.Seek(1, 2).Foreground(11).Frame(82, 27, false);

                    int currentDumpCursor = 0;

                    this.m_Buffer.Seek(2, 3).Anchor();
                    for (int i = 0; i < (h - 6); i++)
                    {
                        int address = currentDumpCursor + i;
                        byte memory = this.Machine.MainMemory[address];

                        this.m_Buffer.MoveFromAnchor(0, i).Foreground(11).Draw($"0x{address:X4}").Foreground(255).Draw($" :: {memory:X2} :: {Convert.ToString(memory, 2).PadLeft(8, '0')}b");
                    }
                    #endregion
                    break;
                case Modes.Instructions:
                    #region Instruction display - displays the current instructin that is run
                    this.m_Buffer.Seek(1, 2).Foreground(11).Frame(82, 27, false);

                    int currentInstructionIndex = this.Machine.Registers.B16[RegisterCodes.PC];
                    int displayMiddle = (h - 2) / 2;

                    this.m_Buffer.Seek(2, 3).Anchor();
                    for (int i = 0; i < (h - 6); i++)
                    {
                        int iteratedInstructionIndex = currentInstructionIndex + ((i - displayMiddle) * 4);
                        if(iteratedInstructionIndex < 0)
                        {

                        }
                        else if(iteratedInstructionIndex > this.Machine.MainMemory.Length - 3)
                        {

                        }   
                        else
                        {
                            byte[] instructinBytes = new byte[]
                            {
                                this.Machine.MainMemory[iteratedInstructionIndex],
                                this.Machine.MainMemory[iteratedInstructionIndex + 1],
                                this.Machine.MainMemory[iteratedInstructionIndex + 2],
                                this.Machine.MainMemory[iteratedInstructionIndex + 3],
                            };
                            MachineInstruction inst = new MachineInstruction(instructinBytes);

                            int background = currentInstructionIndex == iteratedInstructionIndex ? 11 : 0;
                            int foreground = currentInstructionIndex == iteratedInstructionIndex ? 0 : 15;

                            string address = $"0x{iteratedInstructionIndex:X4}";
                            string instruction = $"{inst.Byte1:X2}-{inst.Byte1:X2}-{inst.Byte1:X2}-{inst.Byte1:X2} :: {inst.Opcode}";
                            this.m_Buffer.MoveFromAnchor(0, i).Color(background, foreground).Draw($"{address} :: {instruction}");
                        }
                    }
                    #endregion
                    break;
                case Modes.Test:
                    #region Test display - writes out all colors with id
                    this.m_Buffer.Seek(1, 2).Foreground(118).Frame(82, 27, false);

                    int current = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        this.m_Buffer.Seek(2, i + 3);
                        for (int j = 0; j < 16; j++)
                        {
                            this.m_Buffer.Foreground(current).Draw(current.ToString("000") + " ");
                            current++;
                        }
                    }
                    #endregion
                    break;
            }

            //Draw register box
            this.m_Buffer.Foreground(5).Seek(83, 2).Frame(w - 85, h - 4, false);

            this.m_Buffer.Anchor(85, 3);
            for (int i = 0; i < this.Machine.Registers.Registers.Count; i++)
            {
                string name = this.Machine.Registers.Registers[i].ID.ToString();
                ushort data = this.Machine.Registers.B16[this.Machine.Registers.Registers[i].ID];
                string format = "X" + (this.Machine.Registers.Registers[i].SizeInBytes * 2);

                this.m_Buffer.MoveFromAnchor(0, i).Foreground(14).Draw($"{name.PadRight(5)}: ").Foreground(221).Draw($"0x{data.ToString(format).PadLeft(4, ' ')} ({data})");
            }

            this.m_Buffer.Flush();

        }

        public void Start()
        {
            new Thread(new ThreadStart(this.Runner))
            {
                Name = "[/] Display thread",
                IsBackground = true,
            }.Start();

            new Thread(new ThreadStart(this.KeyboardWatcher))
            {
                Name = "[/] Keyboard thread",
                IsBackground = false,
            }.Start();

        }

        private void Runner()
        {
            while(true)
            {
                this.Draw();
                Thread.Sleep(100);
            }
        }

        private void KeyboardWatcher()
        {
            while (true)
            {
                var info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.F1)
                {
                    this.CurrentMode = Modes.Vram;
                }
                else if (info.Key == ConsoleKey.F2)
                {
                    this.CurrentMode = Modes.Instructions;
                }
                else if (info.Key == ConsoleKey.F3)
                {
                    this.CurrentMode = Modes.MemoryDump;
                }
                else if (info.Key == ConsoleKey.F4)
                {
                    this.CurrentMode = Modes.Test;
                }
                else if(info.Key == ConsoleKey.Spacebar && this.Machine.CurrentMode == RunMode.HandTicked)
                {
                    this.Machine.Ticker.Set();
                }
            }
        }
    }
}
