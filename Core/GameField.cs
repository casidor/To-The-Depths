using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public class GameField
    {
        private char[,] Field;
        public int Width;
        public int Height;
        public GameField(int width, int height )
        {
            this.Width = width;
            this.Height = height;
            Field = new char[height, width];
        }
        public void GenerateField()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (y == 0 || y == Height - 1 || x == 0 || x == Width - 1)
                    {
                        Field[y, x] = '#';
                    }
                    else
                        Field[y, x] = '.';
                }
            }
        }
        public char GetCell(int x, int y)
        {
            return Field[y, x];
        }
        public void SetCell(int x, int y, char value)
        {
            Field[y, x] = value;
        }
    }
}
