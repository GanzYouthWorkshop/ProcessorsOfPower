using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pop.ConsoleUtilities
{
    public class ConsoleBuffer
    {
        private struct Glyph
        {
            public char Character { get; set; }
            public string Specials { get; set; }

            public static implicit operator Glyph(char c)
            {
                return new Glyph(c, null);
            }

            public Glyph(char c, string specials)
            {
                this.Character = c;
                this.Specials = specials;
            }
        }

        public int Width { get; private set; } = 80;
        public int Height { get; private set; } = 25;

        private Glyph[] m_Buffer;
        private Point m_SeekPosition;
        private Point m_Anchor;

        private int m_CurrentBackground = 0;
        private int m_CurrentForeground = 255;

        private int m_SeekIndex
        {
            get { return this.m_SeekPosition.Y * this.Width + this.m_SeekPosition.X; }
        }

        private string GetCurrentSpecials()
        {
            //       | FOREGROUND                           | BACKGROUND                           |
            return $"\u001b[38;5;{this.m_CurrentForeground}m\u001b[48;5;{this.m_CurrentBackground}m";
        }

        public ConsoleBuffer()
        {
            this.m_Buffer = new Glyph[this.Width * this.Height];
            this.m_SeekPosition = new Point(0, 0);
        }

        public ConsoleBuffer Resize(int newWidth, int newHeight)
        {
            Glyph[] newBuffer = new Glyph[newWidth * newHeight];

            int minHeight = Math.Min(this.Height, newHeight);
            int minWidth = Math.Min(this.Width, newWidth);
            for (int i = 0; i < minHeight; i++)
            {
                Array.Copy(this.m_Buffer, i, newBuffer, i, minWidth);
            }

            this.Width = newWidth;
            this.Height = newHeight;
            this.m_Buffer = newBuffer;

            return this;
        }

        public ConsoleBuffer Seek(int x, int y)
        {
            this.m_SeekPosition = new Point(x, y);
            return this;
        }

        public ConsoleBuffer Move(int x, int y)
        {
            this.m_SeekPosition = this.m_SeekPosition + new Point(x, y);
            return this;
        }

        public ConsoleBuffer Background(int color)
        {
            return this.Color(color, this.m_CurrentForeground);
        }

        public ConsoleBuffer Foreground(int color)
        {
            return this.Color(this.m_CurrentBackground, color);
        }

        public ConsoleBuffer Color()
        {
            return this.Color(0, 255);
        }

        public ConsoleBuffer Color(int background, int foreground)
        {
            this.m_CurrentBackground = background;
            this.m_CurrentForeground = foreground;
            return this;
        }

        public ConsoleBuffer Zero()
        {
            this.m_SeekPosition = new Point(0, 0);
            return this;
        }

        public ConsoleBuffer Clip()
        {
            this.m_SeekPosition.X = Math.Min(this.m_SeekPosition.X, this.Width);
            this.m_SeekPosition.X = Math.Max(this.m_SeekPosition.X, 0);
            this.m_SeekPosition.Y = Math.Min(this.m_SeekPosition.Y, this.Height);
            this.m_SeekPosition.Y = Math.Max(this.m_SeekPosition.Y, 0);

            return this;
        }

        public ConsoleBuffer Draw(string s, bool rollover = false)
        {
            bool finished = false;
            char[] charSource = s.ToCharArray();
            Glyph[] source = charSource.Select(c => new Glyph(c, this.GetCurrentSpecials())).ToArray();
            int index = 0;

            while(!finished)
            {
                int length = Math.Min(source.Length, this.Width - this.m_SeekPosition.X);

                Array.Copy(source, index, this.m_Buffer, this.m_SeekIndex, length);

                index += length;
                this.Move(length, 0).Clip();

                if(!rollover || index == source.Length - 1)
                {
                    break;
                }

                this.Seek(0, this.m_SeekPosition.Y + 1);
            }

            return this;
        }

        public ConsoleBuffer Draw(char c, bool rollover = false)
        {
            return this.Draw(new String(c, 1), rollover);
        }

        public ConsoleBuffer Frame(int w, int h, bool doubled)
        {
            //┌─┐ ╔═╗
            //│ │ ║ ║
            //└─┘ ╚═╝

            char ne = doubled ? '╔' : '┌';
            char ns = doubled ? '═' : '─';
            char nw = doubled ? '╗' : '┐';
            char ew = doubled ? '║' : '│';
            char se = doubled ? '╚' : '└';
            char sw = doubled ? '╝' : '┘';

            this.Draw(ne + new String(ns, w - 2) + nw);
            for(int i = 0; i < h - 2; i++)
            {
                this.Move(-w, 1).Draw(ew).Move(w - 2, 0).Draw(ew);
            }
            this.Move(-w, 1).Draw(se + new String(ns, w - 2) + sw);

            return this;
        }

        public ConsoleBuffer Clear()
        {
            for(int i = 0; i < this.m_Buffer.Length; i++)
            {
                this.m_Buffer[i].Character = '\0';
                this.m_Buffer[i].Specials = null;
            }

            return this;
        }

        public ConsoleBuffer Anchor()
        {
            this.m_Anchor = this.m_SeekPosition;
            return this;
        }

        public ConsoleBuffer Anchor(int x, int y)
        {
            this.m_Anchor = new Point(x, y);
            return this;
        }

        public ConsoleBuffer MoveFromAnchor(int x, int y)
        {
            this.m_SeekPosition = this.m_Anchor + new Point(x, y);
            return this;
        }

        public void Flush()
        {
            string lastspecial = "";
            Console.CursorVisible = false;

            StringBuilder sb = new StringBuilder();
            foreach(Glyph g in this.m_Buffer)
            {
                if (!String.IsNullOrEmpty(g.Specials))
                {
                    if (lastspecial != g.Specials)
                    {
                        sb.Append(g.Specials);
                        lastspecial = g.Specials;
                    }
                }
                sb.Append(g.Character);
            }

            string s = sb.ToString();
            s = s.Replace('\0', ' ');
            Console.Write("\u001b[1;1H" + s);
        }
    }
}
