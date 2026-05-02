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
                if (equipped)
                    field.Log.Add(GameEventType.ItemEquipped, $"Equipped: {Name}", ' ', color: LogColor.Good);
                else
                    field.Log.Add(GameEventType.ItemEquipped, "Your weapon is already stronger", ' ', color: LogColor.Bad);
                return equipped ? InteractionResult.ItemPickedUp : InteractionResult.None;
            }
            bool added = player.Inventory.TryAddToHotbar(this);
            if (added)
            {
                field.Log.Add(GameEventType.ItemPickedUp, $"Picked up: {Name}", ' ', color: LogColor.Good);
                field[x, y] = new Floor();
            }
            return added ? InteractionResult.ItemPickedUp : InteractionResult.None;
        }
    }
}
