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
