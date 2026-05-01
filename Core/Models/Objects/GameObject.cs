using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public abstract class GameObject
    {
        public char Symbol { get; protected set; }
        public bool IsPassable { get; protected set; } = false;
        public string Color { get; protected set; } = GameColors.Reset;
        public virtual string SpriteName => GetType().Name.ToLower();
        public abstract InteractionResult Interact(Player player, GameField field, int x, int y);
    }
}
