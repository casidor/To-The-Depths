using GameCore.Interfaces;
using GameCore.Interfaces.GameCore.Interfaces;
using GameCore.Models.Entities;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class Item : GameObject
    {
        public string Name { get; protected set; } = "";
        public string Description { get; protected set; } = "";

        protected Item() => IsPassable = true;

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            if (this is IEquippable eq)
            {
                bool equipped = eq.Equip(player, field, x, y);
                return equipped ? InteractionResult.ItemPickedUp : InteractionResult.None;
            }
            if (this is IUsable us)
            {
                us.Use(player);
                field[x, y] = new Floor();
                return InteractionResult.ItemPickedUp;
            }
            return InteractionResult.None;
        }
    }
}
