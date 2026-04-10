using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public abstract class GameObject
    {
        public char Symbol { get; protected set; }
        public bool IsPassable { get; protected set; } = false;
        public ConsoleColor Color { get; protected set; } = ConsoleColor.White;
        public abstract void Interact(Player player, GameField field, int x, int y);
    }
}
