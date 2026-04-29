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
        public int MaxHP { get; private set; }
        public int HP { get; private set; }
        public int Damage { get; private set; } = Config.PlayerDamage;
        public bool IsAlive => HP > 0;
        public int KeysCollected { get; private set; } = 0;
        public int CurrentFloor { get; private set; } = 1;
        private int _gold;
        public int GoldCollected
        {
            get => _gold;
            private set => _gold = Math.Max(value, 0);
        }
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
        public Player(int x, int y, int hp,int maxhp, int gold, int keys, int floor)
        {
            X = x;
            Y = y;
            HP = hp;
            MaxHP = maxhp;
            GoldCollected = gold;
            KeysCollected = keys;
            CurrentFloor = floor;
        }
        public InteractionResult Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            var cell = field[newX, newY];
            var result = cell.Interact(this, field, newX, newY);
            if (field[newX, newY].IsPassable)
            {
                X = newX;
                Y = newY;
            }
            return result;
        }
        public void Descend(int startX, int startY)
        {
            X = startX;
            Y = startY;
            KeysCollected = 0;
            IsExited = false;
            CurrentFloor++;
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
        public bool SpendGold(int amount)
        {
            if (GoldCollected < amount) return false;
            GoldCollected -= amount;
            return true;
        }
        public bool Heal(int amount)
        {
            if (HP >= MaxHP) return false;
            HP = Math.Min(HP + amount, MaxHP);
            return true;
        }
    }
}
