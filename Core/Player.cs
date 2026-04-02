using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public class Player
    {
        public int X;
        public int Y;
        public Player(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            if(field.GetCell(newX, newY) != GameSymbols.Wall)
            {
                X = newX;
                Y = newY;
            }
        }
    }
}
