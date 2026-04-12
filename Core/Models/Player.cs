using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Player
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public bool IsAlive => HP > 0;
        public int KeysCollected { get; private set; } = 0;
        public int GoldCollected { get; private set; } = 0;
        public bool IsExited { get; private set; } = false;
        public char Symbol { get; private set; } = GameSymbols.Player;
        public string Color { get; private set; } = GameColors.Player;
        public Player(int x, int y)
        {
            X = x;
            Y = y;
            HP = Config.PlayerMaxHP;
            MaxHP = Config.PlayerMaxHP;
        }
        public void Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            var cell = field[newX, newY];
            cell.Interact(this, field, newX, newY);
            if (field[newX, newY].IsPassable)
            {
                X = newX;
                Y = newY;
            }
        }
        public void AddGold(int amount)
        {
            GoldCollected += amount;
        }
        public void AddKey()
        {
            KeysCollected++;
        }
        public void Exit()
        {
            IsExited = true;
        }
        public void TakeDamage(int damage)
        {
            HP = Math.Max(HP - damage, 0);
        }
    }
}
