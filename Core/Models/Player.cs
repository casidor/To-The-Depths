using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Player : Entity
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int KeysCollected { get; private set; } = 0;
        public int GoldCollected { get; private set; } = 0;
        public bool IsExited { get; private set; } = false;
        public char Symbol { get; private set; } = GameSymbols.Player;
        public string Color { get; private set; } = GameColors.Player;
        public Enemy? PendingEnemy { get; private set; }
        public Player(int x, int y)
        {
            X = x;
            Y = y;
            Name = "Player";
            HP = Config.PlayerMaxHP;
            MaxHP = Config.PlayerMaxHP;
            Attack = Config.PlayerAttack;
        }
        public void StartBattle(Enemy enemy)
        {
            PendingEnemy = enemy;
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
    }
}
