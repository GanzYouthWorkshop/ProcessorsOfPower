using Pop81.VM.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pop81.VM
{
    using CD = ConsoleDraw;

    public class Display
    {
        public const int VRAM_START = 5000;

        public const int WIDTH = 200;
        public const int HEIGHT = 50;


        public VirtualMachine Machine { get; set; }
        public Modes CurrentMode { get; set; }

        public enum Modes
        {
            Vram,
            MemoryDump,
            Instructions,
        }

        public void Draw()
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;

            string result = "";
            result += this.DrawHeader();
            result += this.DrawFrame(w, h);
            //this.DrawFooter();

            Console.Write(result);

            switch (this.CurrentMode)
            {
                case Modes.Vram: break;
                case Modes.MemoryDump: break;
                case Modes.Instructions: break;
            }
        }

        public string DrawHeader()
        {
            return $"{CD.Position(0, 0)} Pop81 VM {Machine.CurrentMode}{CD.ClearRemainingLine()}";
        }

        public string DrawFrame(int width, int height)
        {
            //┌─┐
            //│ │
            //└─┘
            string result = $"{CD.Position(0, 1)}" + '┌' + new String('─', width - 2) + '┐';

            for(int y = 2; y < height - 2; y++)
            {
                result += $"{CD.Position(0, y)}│{CD.Position(width - 1, y)}│";
            }
            result += '└' + new String('─', width - 2) + '┘';

            return result;
        }

        public void Start()
        {
            new Thread(new ThreadStart(this.Runner))
            {
                Name = "[/] Display thread",
                IsBackground = true,
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
    }


    public static class ConsoleDraw
    {
        public static string Position(int x, int y)
        {
            return $"\u001b[{y + 1};{x + 1}H";
        }

        public static string ClearLine()
        {
            return $"\u001b[2K";
        }

        public static string ClearRemainingLine()
        {
            return $"\u001b[0K";
        }

        public static string Left(int count)
        {
            return $"\u001b[{count}D";
        }
    }
}
