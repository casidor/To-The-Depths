using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public class Player
    {
        public int X;
        public int Y;
        private int KeysCollected = 0;
        public int Score = 0;
        public bool isExited = false;
        public Player(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            char cell = field.GetCell(newX, newY);
            bool canMove = false;
            switch (cell)
            {
                case GameSymbols.Wall:
                    break;
                case GameSymbols.Key:
                    KeysCollected++;
                    canMove = true;
                    field.SetCell(newX, newY, GameSymbols.Floor);
                    break;
                case GameSymbols.Gold:
                    Score += 10;
                    canMove = true;
                    field.SetCell(newX, newY, GameSymbols.Floor);
                    break;
                case GameSymbols.Exit:
                    if(KeysCollected == 3)
                    {
                        isExited = true;
                    }
                    canMove = true;
                    break;
                default:
                    canMove = true;
                    break;
            }
            if (canMove)
            {
                X = newX;
                Y = newY;
            }
        }
    }
}
