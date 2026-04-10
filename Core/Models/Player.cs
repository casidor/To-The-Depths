using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Player 
    {
        public int X;
        public int Y;
        public int KeysCollected = 0;
        public int GoldCollected = 0;
        public bool isExited = false;
        public char Symbol = GameSymbols.Player;
        public Player(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            var cell = field[newX, newY];
            bool canMove = false;
            switch (cell)
            {
                case Wall:
                    break;
                case Key:
                    KeysCollected++;
                    canMove = true;
                    field[newX, newY] = new Floor();
                    break;
                case Gold:
                    GoldCollected += Config.GoldAmount;
                    canMove = true;
                    field[newX, newY] = new Floor();
                    break;
                case Exit:
                    if(KeysCollected == Config.KeysAmount)
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
